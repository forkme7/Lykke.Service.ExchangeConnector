﻿using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Newtonsoft.Json;
using Polly;

namespace Lykke.ExternalExchangesApi.Shared
{
    public sealed class WebSocketTextMessenger : IMessenger<object, string>
    {
        private readonly string _endpointUrl;
        private readonly ILog _log;
        private ClientWebSocket _clientWebSocket;
        private readonly TimeSpan _responseTimeout = TimeSpan.FromSeconds(10);

        public WebSocketTextMessenger(string endpointUrl, ILog log)
        {

            _endpointUrl = endpointUrl;
            _log = log;
        }

        public void Dispose()
        {
            _clientWebSocket?.Dispose();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await _log.WriteInfoAsync(nameof(ConnectAsync), "Connecting to WebSocket", $"API endpoint {_endpointUrl}");
            var uri = new Uri(_endpointUrl);

            const int attempts = 20;
            var retryPolicy = Policy
                .Handle<Exception>(e => !cancellationToken.IsCancellationRequested)
                .WaitAndRetryAsync(attempts, attempt => TimeSpan.FromSeconds(3));
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    try
                    {
                        _clientWebSocket = new ClientWebSocket();
                        using (var connectionTimeoutCts = new CancellationTokenSource(_responseTimeout))
                        using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, connectionTimeoutCts.Token))
                        {
                            await _clientWebSocket.ConnectAsync(uri, linkedCts.Token);
                        }
                        await _log.WriteInfoAsync(nameof(ConnectAsync), "Successfully connected to WebSocket", $"API endpoint {_endpointUrl}");
                    }
                    catch (Exception ex)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await _log.WriteWarningAsync(nameof(ConnectAsync), $"Unable to connect to {_endpointUrl}. Retry in 3 sec.", ex.Message);
                        }
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    await _log.WriteErrorAsync(nameof(ConnectAsync),
                        $"Unable to connect to {_endpointUrl} after {attempts} attempts", ex);
                }
                throw;
            }
        }


        public async Task SendRequestAsync(object request, CancellationToken cancellationToken)
        {
            try
            {
                var msg = EncodeRequest(request);
                using (var connectionTimeoutCts = new CancellationTokenSource(_responseTimeout))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, connectionTimeoutCts.Token))
                {
                    await _clientWebSocket.SendAsync(msg, WebSocketMessageType.Text, true, linkedCts.Token);
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(ConnectAsync), "An exception occurred while sending request", ex);
                throw;
            }
        }


        public async Task<string> GetResponseAsync(CancellationToken cancellationToken)
        {
            using (var cts = new CancellationTokenSource(_responseTimeout))
            using (var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken))
            {

                var buffer = new byte[10000];
                var segment = new ArraySegment<byte>(buffer);
                var sb = new StringBuilder();
                var endOfMessage = false;
                while (!endOfMessage)
                {
                    var re = await _clientWebSocket.ReceiveAsync(segment, linkedToken.Token);
                    sb.Append(DecodeText(segment.Array.Take(re.Count).ToArray()));
                    endOfMessage = re.EndOfMessage;
                }
                return sb.ToString();
            }
        }



        private static ArraySegment<byte> EncodeRequest(object request)
        {
            return new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request)));
        }

        private static string DecodeText(byte[] message)
        {
            return Encoding.UTF8.GetString(message);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
            {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Good bye", cancellationToken);
            }
        }
    }
}
