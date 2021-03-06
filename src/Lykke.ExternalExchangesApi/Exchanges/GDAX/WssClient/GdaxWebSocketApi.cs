﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.ExternalExchangesApi.Exceptions;
using Lykke.ExternalExchangesApi.Exchanges.GDAX.Credentials;
using Lykke.ExternalExchangesApi.Exchanges.GDAX.RestClient.Entities;
using Lykke.ExternalExchangesApi.Exchanges.GDAX.WssClient.Entities;
using Lykke.ExternalExchangesApi.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lykke.ExternalExchangesApi.Exchanges.GDAX.WssClient
{
    public class GdaxWebSocketApi: IDisposable
    {
        public const string GdaxPublicWssApiUrl = @"wss://ws-feed.gdax.com";
        public const string GdaxSandboxWssApiUrl = @"wss://ws-feed-public.sandbox.gdax.com";
        private const string _selfVerifyUrl = @"/users/self/verify";
        private const string _tickerChannelName = "ticker";
        private const string _fullChannelName = "full";
        private const string _userChannelName = "user";
        private const string _subscribeActionName = "subscribe";

        private readonly ILog _logger;
        private readonly GdaxCredentialsFactory _credentialsFactory;
        private ClientWebSocket _clientWebSocket;

        /// <summary>
        /// Raised on established WebSocket connection with the server
        /// </summary>
        public AsyncEvent<Uri> Connected;

        /// <summary>
        /// Raised when WebSocket connection is lost
        /// </summary>
        public AsyncEvent<Uri> Disconnected;

        /// <summary>
        /// Raised on successful subscription
        /// </summary>
        public AsyncEvent<string> Subscribed;

        /// <summary>
        /// Raised when a valid order has been received and is now active. This message 
        /// is emitted for every single valid order as soon as the matching engine receives 
        /// it whether it fills immediately or not.
        /// </summary>
        public AsyncEvent<GdaxWssOrderReceived> OrderReceived;

        /// <summary>
        /// Raised when the order is now open on the order book. This message will only be 
        /// sent for orders which are not fully filled immediately. RemainingSize will 
        /// indicate how much of the order is unfilled and going on the book
        /// </summary>
        public AsyncEvent<GdaxWssOrderOpen> OrderOpened;

        /// <summary>
        /// Raised when the order is no longer on the order book. Sent for all orders for which there 
        /// was a received message. This message can result from an order being canceled or filled. 
        /// There will be no more messages for this OrderId after a done message. RemainingSize indicates 
        /// how much of the order went unfilled; this will be 0 for filled orders.
        /// Market orders will not have a remaining_size or price field as they are never on the 
        /// open order book at a given price.
        /// </summary>
        public AsyncEvent<GdaxWssOrderDone> OrderDone;

        /// <summary>
        /// Raised when a trade occurred between two orders. The aggressor or taker order is the one 
        /// executing immediately after being received and the maker order is a resting order on the book. 
        /// The side field indicates the maker order side. If the side is sell this indicates the maker 
        /// was a sell order and the match is considered an up-tick. A buy side match is a down-tick.
        /// </summary>
        public AsyncEvent<GdaxWssOrderMatch> OrderMatched;

        /// <summary>
        /// Raised when an order has changed. This is the result of self-trade prevention adjusting the 
        /// order size or available funds. Orders can only decrease in size or funds. Change messages are 
        /// sent anytime an order changes in size; this includes resting orders (open) as well as received 
        /// but not yet open. Change messages are also sent when a new market order goes through self trade 
        /// prevention and the funds for the market order have changed.
        /// </summary>
        public AsyncEvent<GdaxWssOrderChange> OrderChanged;

        /// <summary>
        /// The ticker provides real-time price updates every time a match happens. It batches updates 
        /// in case of cascading matches, greatly reducing bandwidth requirements.
        /// </summary>
        public AsyncEvent<GdaxWssTicker> Ticker;

        /// <summary>
        /// Most failure cases will cause an error message (a message with the type "error") to be emitted. 
        /// This can be helpful for implementing a client or debugging issues.
        /// </summary>
        public AsyncEvent<GdaxWssError> Error;

        /// <summary>
        /// Base GDAX WebSockets Uri
        /// </summary>
        public Uri BaseUri { get; set; }

        public GdaxWebSocketApi(ILog logger, string apiKey, string apiSecret, string passPhrase) :
             this(logger, apiKey, apiSecret, passPhrase, GdaxPublicWssApiUrl)
        { }

        public GdaxWebSocketApi(ILog logger, string apiKey, string apiSecret, string passPhrase,
            string publicApiUrl)
        {
            _logger = logger;
            _credentialsFactory = new GdaxCredentialsFactory(apiKey, apiSecret, passPhrase);

            BaseUri = new Uri(publicApiUrl);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await _logger.WriteInfoAsync(nameof(GdaxWebSocketApi), nameof(ConnectAsync), BaseUri.ToString(),
                "Attempt to establish a socket connection");
            
            _clientWebSocket = new ClientWebSocket();
            await _clientWebSocket.ConnectAsync(BaseUri, cancellationToken).ConfigureAwait(false);

            if (_clientWebSocket.State != WebSocketState.Open)
                throw new ApiException($"Could not establish WebSockets connection to {BaseUri}");

            await Connected.NullableInvokeAsync(this, BaseUri);
        }

        public async Task SubscribeToPrivateUpdatesAsync(IReadOnlyCollection<string> productIds, 
            CancellationToken cancellationToken)
        {
            ThrowOnClosedConnection();

            var requestString = GetCredentialSubscriptionMessage(
                new[] {
                    new GdaxSubscriptionChannel { Name = _tickerChannelName, ProductIds = productIds },
                    new GdaxSubscriptionChannel { Name = _userChannelName, ProductIds = productIds }
                });

            await SubscribeAndListenImplAsync(cancellationToken, requestString);
        }

        public async Task SubscribeToOrderBookUpdatesAsync(IReadOnlyCollection<string> productIds, 
            CancellationToken cancellationToken)
        {
            ThrowOnClosedConnection();

            var subscriptionChannels = new[] {
                new GdaxSubscriptionChannel { Name = _tickerChannelName, ProductIds = productIds },
                new GdaxSubscriptionChannel { Name = _fullChannelName, ProductIds = productIds }
            };

            var requestString = string.IsNullOrEmpty(_credentialsFactory.ApiKey)
                ? GetAnonymousSubscriptionMessage(subscriptionChannels)
                : GetCredentialSubscriptionMessage(subscriptionChannels);

            await SubscribeAndListenImplAsync(cancellationToken, requestString);
        }

        private async Task SubscribeAndListenImplAsync(CancellationToken cancellationToken, string requestString)
        {
            await _clientWebSocket.SendAsync(StringToArraySegment(requestString), WebSocketMessageType.Text,
                true, cancellationToken).ConfigureAwait(false);

            await Subscribed.NullableInvokeAsync(this, requestString);

            await ListenToMessagesAsync(_clientWebSocket, cancellationToken);
        }

        private async Task ListenToMessagesAsync(ClientWebSocket webSocket, CancellationToken cancellationToken)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                using (var stream = new MemoryStream(8192))
                {
                    var receiveBuffer = new ArraySegment<byte>(new byte[1024]);
                    WebSocketReceiveResult receiveResult;
                    do
                    {
                        receiveResult = await webSocket.ReceiveAsync(receiveBuffer,
                            cancellationToken);
                        await stream.WriteAsync(receiveBuffer.Array, receiveBuffer.Offset, receiveResult.Count, 
                            cancellationToken);
                    } while (!receiveResult.EndOfMessage);

                    var messageBytes = stream.ToArray();
                    var jsonMessage = Encoding.UTF8.GetString(messageBytes, 0, messageBytes.Length);

                    await HandleWebSocketMessageAsync(jsonMessage);
                }
            }
        }

        public async Task CloseConnectionAsync(CancellationToken cancellationToken)
        {
            if (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
            {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal closure", cancellationToken);
                await Disconnected.NullableInvokeAsync(this, BaseUri);
            }
        }

        private ArraySegment<byte> StringToArraySegment(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var messageArraySegment = new ArraySegment<byte>(messageBytes);
            return messageArraySegment;
        }

        private async Task HandleWebSocketMessageAsync(string jsonMessage)
        {
            var jToken = JToken.Parse(jsonMessage);
            var type = jToken["type"]?.Value<string>();

            switch (type)
            {
                case "received":
                    var orderReceived = JsonConvert.DeserializeObject<GdaxWssOrderReceived>(jsonMessage);
                    await OrderReceived.NullableInvokeAsync(this, orderReceived);
                    break;
                case "open":
                    var orderOpen = JsonConvert.DeserializeObject<GdaxWssOrderOpen>(jsonMessage);
                    await OrderOpened.NullableInvokeAsync(this, orderOpen);
                    break;
                case "done":
                    var orderDone = JsonConvert.DeserializeObject<GdaxWssOrderDone>(jsonMessage);
                    await OrderDone.NullableInvokeAsync(this, orderDone);
                    break;
                case "match":
                    var orderMatch = JsonConvert.DeserializeObject<GdaxWssOrderMatch>(jsonMessage);
                    await OrderMatched.NullableInvokeAsync(this, orderMatch);
                    break;
                case "change":
                    var orderChange = JsonConvert.DeserializeObject<GdaxWssOrderChange>(jsonMessage);
                    await OrderChanged.NullableInvokeAsync(this, orderChange);
                    break;
                case "ticker":
                    var tickerDetails = JsonConvert.DeserializeObject<GdaxWssTicker>(jsonMessage);
                    await Ticker.NullableInvokeAsync(this, tickerDetails);
                    break;
                case "error":
                    var error = JsonConvert.DeserializeObject<GdaxWssError>(jsonMessage);
                    await Error.NullableInvokeAsync(this, error);
                    break;
                default:
                    // Clients are expected to ignore messages they do not support.
                    break;
            }
        }

        private GdaxCredentials GenerateClientCredentials()
        {
            return _credentialsFactory.GenerateCredentials(HttpMethod.Get,
                new Uri(BaseUri, _selfVerifyUrl), string.Empty);
        }

        private void ThrowOnClosedConnection()
        {
            if (_clientWebSocket == null || _clientWebSocket.State != WebSocketState.Open)
                throw new ApiException($"Could not subscribe to {BaseUri} because no connection is established.");
        }

        private string GetAnonymousSubscriptionMessage(
            IReadOnlyCollection<GdaxSubscriptionChannel> subscriptionChannels)
        {
            return JsonConvert.SerializeObject(new
            {
                type = _subscribeActionName,
                channels = subscriptionChannels
            });
        }

        private string GetCredentialSubscriptionMessage(
            IReadOnlyCollection<GdaxSubscriptionChannel> subscriptionChannels)
        {
            var credentials = GenerateClientCredentials();
            return JsonConvert.SerializeObject(new
            {
                type = _subscribeActionName,
                signature = credentials.Signature,
                key = credentials.ApiKey,
                passphrase = credentials.PassPhrase,
                timestamp = credentials.UnixTimestampString,
                channels = subscriptionChannels
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GdaxWebSocketApi()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (_clientWebSocket != null)
                {
                    _clientWebSocket.Abort();
                    _clientWebSocket.Dispose();
                    _clientWebSocket = null;
                }
            }
        }
    }
}
