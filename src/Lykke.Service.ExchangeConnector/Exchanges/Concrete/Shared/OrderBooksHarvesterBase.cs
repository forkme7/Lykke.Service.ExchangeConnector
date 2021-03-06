﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Polly;
using TradingBot.Communications;
using TradingBot.Handlers;
using TradingBot.Infrastructure.Configuration;
using TradingBot.Infrastructure.Exceptions;
using TradingBot.Trading;

namespace TradingBot.Exchanges.Concrete.Shared
{
    internal abstract class OrderBooksHarvesterBase : IDisposable
    {
        protected readonly ILog Log;
        protected readonly OrderBookSnapshotsRepository OrderBookSnapshotsRepository;
        protected readonly OrderBookEventsRepository OrderBookEventsRepository;
        protected CancellationToken CancellationToken;

        private readonly ConcurrentDictionary<string, OrderBookSnapshot> _orderBookSnapshots;
        private readonly ExchangeConverters _converters;
        private readonly Timer _heartBeatMonitoringTimer;
        protected TimeSpan HeartBeatPeriod { get; set; } = TimeSpan.FromSeconds(30);
        private CancellationTokenSource _cancellationTokenSource;
        private Task _messageLoopTask;
        private readonly IHandler<OrderBook> _newOrderBookHandler;
        private DateTime _lastPublishTime = DateTime.MinValue;
        private long _lastSecPublicationsNum;
        private int _orderBooksReceivedInLastTimeFrame;
        private Task _measureTask;
        private long _publishedToRabbit;

        protected IExchangeConfiguration ExchangeConfiguration { get; }

        public string ExchangeName { get; }

        public int MaxOrderBookRate { get; set; }

        protected OrderBooksHarvesterBase(string exchangeName, IExchangeConfiguration exchangeConfiguration, ILog log,
            OrderBookSnapshotsRepository orderBookSnapshotsRepository, OrderBookEventsRepository orderBookEventsRepository,
            IHandler<OrderBook> newOrderBookHandler)
        {
            ExchangeConfiguration = exchangeConfiguration;
            OrderBookSnapshotsRepository = orderBookSnapshotsRepository;
            OrderBookEventsRepository = orderBookEventsRepository;
            _newOrderBookHandler = newOrderBookHandler;
            ExchangeName = exchangeName;

            Log = log.CreateComponentScope(GetType().Name);

            _converters = new ExchangeConverters(exchangeConfiguration.SupportedCurrencySymbols,
                string.Empty);

            _orderBookSnapshots = new ConcurrentDictionary<string, OrderBookSnapshot>();
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;

            _heartBeatMonitoringTimer = new Timer(RestartMessenger);
        }

        private async void RestartMessenger(object state)
        {
            await Log.WriteWarningAsync(nameof(RestartMessenger), "Monitoring heartbeat",
                $"Heart stopped. Restarting {GetType().Name}");
            Stop();
            try
            {
                await _messageLoopTask;
            }
            catch (OperationCanceledException)
            {

            }
            Start();
        }

        protected void RechargeHeartbeat()
        {
            _heartBeatMonitoringTimer.Change(HeartBeatPeriod, Timeout.InfiniteTimeSpan);
        }

        private async Task Measure()
        {
            const double period = 60;
            while (!CancellationToken.IsCancellationRequested)
            {
                var msgInSec = _lastSecPublicationsNum / period;
                var pubInSec = _publishedToRabbit / period;
                await Log.WriteInfoAsync(nameof(OrderBooksHarvesterBase),
                    $"Receive rate from {ExchangeName} {msgInSec} per second, publish rate to " +
                    $"RabbitMq {pubInSec} per second", string.Empty);
                _lastSecPublicationsNum = 0;
                _publishedToRabbit = 0;
                await Task.Delay(TimeSpan.FromSeconds(period), CancellationToken).ConfigureAwait(false);
            }
        }

        public virtual void Start()
        {
            Log.WriteInfoAsync(nameof(Start), "Starting", $"Starting {GetType().Name}").Wait();

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;
            _measureTask = Task.Run(Measure, _cancellationTokenSource.Token);
            StartReading();
        }

        protected virtual void StartReading()
        {
            _messageLoopTask = Task.Run(MessageLoop, CancellationToken);
        }

        public virtual void Stop()
        {
            Log.WriteInfoAsync(nameof(Stop), "Stopping", $"Stopping {GetType().Name}").Wait();
            _cancellationTokenSource?.Cancel();
            SwallowCanceledException(() =>
                _messageLoopTask?.GetAwaiter().GetResult());
            SwallowCanceledException(() =>
                _measureTask?.GetAwaiter().GetResult());
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async Task MessageLoop()
        {
            const int smallTimeout = 5;
            var retryPolicy = Policy
                .Handle<Exception>(ex => !CancellationToken.IsCancellationRequested)
                .WaitAndRetryForeverAsync(attempt =>
                {
                    if (attempt % 60 == 0)
                    {
                        Log.WriteErrorAsync("Receiving messages from the socket", "Unable to recover the connection after 60 attempts. Will try in 5 min. ", null).GetAwaiter().GetResult();
                    }
                    return attempt % 60 == 0
                        ? TimeSpan.FromMinutes(5)
                        : TimeSpan.FromSeconds(smallTimeout);
                }); // After every 60 attempts wait 5min 

            await retryPolicy.ExecuteAsync(async () =>
             {
                 await Log.WriteInfoAsync(nameof(MessageLoopImpl), "Starting message loop", "");
                 try
                 {
                     await MessageLoopImpl();
                 }
                 catch (OperationCanceledException)
                 {
                     throw;
                 }
                 catch (Exception ex)
                 {
                     await Log.WriteErrorAsync(nameof(MessageLoopImpl),
                         $"An exception occurred while working with WebSocket. Reconnect in {smallTimeout} sec", ex);
                     throw;
                 }
             });
        }

        private async Task PublishOrderBookSnapshotAsync(string pair)
        {
            _lastSecPublicationsNum++;
            if (NeedThrottle())
            {
                return;
            }

            var obs = _orderBookSnapshots[pair];
            var orderBook = new OrderBook(
                     ExchangeName,
                     _converters.ExchangeSymbolToLykkeInstrument(obs.AssetPair).Name,
                     obs.Asks.Values.Select(i => new VolumePrice(i.Price, i.Size)).ToArray(),
                     obs.Bids.Values.Select(i => new VolumePrice(i.Price, i.Size)).ToArray(),
                     DateTime.UtcNow);
            _publishedToRabbit++;

            if (orderBook.Asks.Any() || orderBook.Bids.Any())
            {
                await _newOrderBookHandler.Handle(orderBook);
            }

            else
            {
                await Log.WriteInfoAsync(nameof(PublishOrderBookSnapshotAsync), $"Source: {orderBook.Source} AssetPairId: {orderBook.AssetPairId}", "skip empty order book");
            }

        }

        protected abstract Task MessageLoopImpl();

        private async Task<OrderBookSnapshot> GetOrderBookSnapshot(string pair)
        {
            if (!_orderBookSnapshots.TryGetValue(pair, out var orderBook))
            {
                var message = "Trying to retrieve a non-existing pair order book snapshot " +
                              $"for exchange {ExchangeName} and pair {pair}";
                await Log.WriteErrorAsync(nameof(MessageLoopImpl), nameof(MessageLoopImpl),
                    new OrderBookInconsistencyException(message));
                throw new OrderBookInconsistencyException(message);
            }

            return orderBook;
        }

        protected bool TryGetOrderBookSnapshot(string pair, out OrderBookSnapshot orderBookSnapshot)
        {
            return _orderBookSnapshots.TryGetValue(pair, out orderBookSnapshot);
        }

        protected async Task HandleOrdebookSnapshotAsync(string pair, DateTime timeStamp, IEnumerable<OrderBookItem> orders)
        {
            var orderBookSnapshot = new OrderBookSnapshot(ExchangeName, pair, timeStamp);
            orderBookSnapshot.AddOrUpdateOrders(orders);

            if (ExchangeConfiguration.SaveOrderBooksToAzure)
                await OrderBookSnapshotsRepository.SaveAsync(orderBookSnapshot);

            _orderBookSnapshots[pair] = orderBookSnapshot;

            await PublishOrderBookSnapshotAsync(pair);
        }

        protected async Task HandleOrdersEventsAsync(string pair,
            OrderBookEventType orderEventType,
            IReadOnlyCollection<OrderBookItem> orders)
        {
            var orderBookSnapshot = await GetOrderBookSnapshot(pair);

            switch (orderEventType)
            {
                case OrderBookEventType.Add:
                case OrderBookEventType.Update:
                    orderBookSnapshot.AddOrUpdateOrders(orders);
                    break;
                case OrderBookEventType.Delete:
                    orderBookSnapshot.DeleteOrders(orders);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderEventType), orderEventType, null);
            }

            if (ExchangeConfiguration.SaveOrderBooksToAzure)
            {
                await OrderBookEventsRepository.SaveAsync(new OrderBookEvent
                {
                    SnapshotId = orderBookSnapshot.GeneratedId,
                    EventType = orderEventType,
                    OrderEventTimestamp = DateTime.UtcNow,
                    OrderItems = orders
                });
            }

            await PublishOrderBookSnapshotAsync(pair);
        }

        private bool NeedThrottle()
        {
            var result = false;
            if (MaxOrderBookRate == 0)
            {
                return result;
            }
            if (_orderBooksReceivedInLastTimeFrame >= MaxOrderBookRate)
            {
                var now = DateTime.UtcNow;
                if ((now - _lastPublishTime).TotalSeconds >= 1)
                {
                    _orderBooksReceivedInLastTimeFrame = 0;
                    _lastPublishTime = now;
                }
                else
                {
                    result = true;
                }
            }
            _orderBooksReceivedInLastTimeFrame++;
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OrderBooksHarvesterBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                Stop();
                _messageLoopTask?.Dispose();
                _heartBeatMonitoringTimer?.Dispose();
                _measureTask?.Dispose();
            }
        }

        private void SwallowCanceledException(Action action)
        {
            try
            {
                action();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
