﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Polly;
using TradingBot.Communications;
using TradingBot.Exchanges.Concrete.BitMEX;
using TradingBot.Infrastructure.Configuration;
using TradingBot.Infrastructure.Exceptions;
using TradingBot.Trading;

namespace TradingBot.Exchanges.Concrete.Shared
{
    internal abstract class OrderBooksHarvesterBase : IDisposable
    {
        protected readonly ILog Log;
        protected readonly ConcurrentDictionary<string, OrderBookSnapshot> OrderBookSnapshots;
        protected CancellationToken CancellationToken;
        protected readonly OrderBookSnapshotsRepository OrderBookSnapshotsRepository;
        protected readonly OrderBookEventsRepository OrderBookEventsRepository;

        private Task _messageLoopTask;
        private Func<OrderBook, Task> _newOrderBookHandler;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private DateTime _lastPublishTime = DateTime.UtcNow;
        private long _lastSecPublicationsNum;
        private int _currentPublicationsNum;
        private long _currentPublicationsNumPerfCounter;

        protected ICurrencyMappingProvider CurrencyMappingProvider { get; }

        public string ExchangeName { get; set; }

        public int MaxOrderBookRate { get; set; }

        protected OrderBooksHarvesterBase(ICurrencyMappingProvider currencyMappingProvider, ILog log,
            OrderBookSnapshotsRepository orderBookSnapshotsRepository, OrderBookEventsRepository orderBookEventsRepository)
        {
            CurrencyMappingProvider = currencyMappingProvider;
            OrderBookSnapshotsRepository = orderBookSnapshotsRepository;
            OrderBookEventsRepository = orderBookEventsRepository;

            Log = log.CreateComponentScope(GetType().Name);

            OrderBookSnapshots = new ConcurrentDictionary<string, OrderBookSnapshot>();
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;

            new Task(ct => Measure((CancellationToken)ct), CancellationToken)
                .Start();
        }

        private async void Measure(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var msgInSec = (_currentPublicationsNumPerfCounter - _lastSecPublicationsNum) / 10d;
                await Log.WriteInfoAsync(nameof(OrderBooksHarvesterBase), 
                    $"Order books received from {ExchangeName} in 1 sec", msgInSec.ToString());
                _lastSecPublicationsNum = _currentPublicationsNumPerfCounter;
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }

        public void AddHandler(Func<OrderBook, Task> handler)
        {
            _newOrderBookHandler = handler;
        }

        public void Start()
        {
            _messageLoopTask = new Task(MessageLoop);
            _messageLoopTask.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private async void MessageLoop()
        {
            const int smallTimeout = 5;
            var retryPolicy = Policy
                .Handle<Exception>(ex => !(ex is OperationCanceledException))
                .WaitAndRetryForeverAsync(attempt => attempt % 60 == 0 
                    ? TimeSpan.FromMinutes(5) 
                    : TimeSpan.FromSeconds(smallTimeout)); // After every 60 attempts wait 5min 

            await retryPolicy.ExecuteAsync(async () =>
            {
                await Log.WriteInfoAsync(nameof(MessageLoopImpl), "Starting message loop", "");
                try
                {
                    await MessageLoopImpl();
                }
                catch (Exception ex)
                {
                    await Log.WriteErrorAsync(nameof(MessageLoopImpl), 
                        $"An exception occurred while working with WebSocket. Reconnect in {smallTimeout} sec", ex);
                    throw;
                }
            });
        }

        protected async Task PublishOrderBookSnapshotAsync()
        {
            _currentPublicationsNumPerfCounter++;
            if (NeedThrottle())
            {
                return;
            }

            var orderBooks = OrderBookSnapshots.Values
                .Select(obs => new OrderBook(
                    ExchangeName,
                    ConvertSymbolFromExchangeToLykke(obs.AssetPair).Name,
                    obs.Asks.Values.Select(i => new VolumePrice(i.Price, i.Size)).ToArray(),
                    obs.Bids.Values.Select(i => new VolumePrice(i.Price, i.Size)).ToArray(),
                    DateTime.UtcNow));

            foreach (var orderBook in orderBooks)
            {
                await _newOrderBookHandler(orderBook);
            }
        }

        protected abstract Task MessageLoopImpl();

        protected async Task<OrderBookSnapshot> GetOrderBookSnapshot(string pair)
        {
            if (!OrderBookSnapshots.TryGetValue(pair, out var orderBook))
            {
                var message = "Trying to retrieve a non-existing pair order book snapshot " +
                              $"for exchange {ExchangeName} and pair {pair}";
                await Log.WriteInfoAsync(nameof(MessageLoopImpl), nameof(MessageLoopImpl), message);
                throw new OrderBookInconsistencyException(message);
            }

            return orderBook;
        }

        protected async Task<ConcurrentDictionary<string, OrderBookItem>> GetOrdersList(string pair, bool isBuy)
        {
            var orderBookSnapshot = await GetOrderBookSnapshot(pair);
            return isBuy
                ? orderBookSnapshot.Bids
                : orderBookSnapshot.Asks;
        }

        protected async Task HandleOrdebookSnapshotAsync(string pair, DateTime timeStamp, 
            IEnumerable<OrderBookItem> orders)
        {
            var orderBookSnapshot = new OrderBookSnapshot(ExchangeName, pair, timeStamp);
            AddOrUpdateOrders(orderBookSnapshot, orders);

            await OrderBookSnapshotsRepository.SaveAsync(orderBookSnapshot);

            await PublishOrderBookSnapshotAsync();
        }

        protected async Task HandleOrdersEventsAsync(string pair, 
            OrderBookEventType orderEventType,
            ICollection<OrderBookItem> orders)
        {
            var orderBookSnapshot = await GetOrderBookSnapshot(pair);

            await OrderBookEventsRepository.SaveAsync(new OrderBookEvent
            {
                SnapshotId = orderBookSnapshot.GeneratedId,
                EventType = orderEventType,
                OrderEventTimestamp = DateTime.UtcNow,
                OrderItems = orders
            });

            switch (orderEventType)
            {
                case OrderBookEventType.Add:
                case OrderBookEventType.Update:
                    AddOrUpdateOrders(orderBookSnapshot, orders);
                    break;
                case OrderBookEventType.Delete:
                    foreach (var order in orders)
                    {
                        if (order.IsBuy)
                            orderBookSnapshot.Bids.TryRemove(order.Id, out var _);
                        else
                            orderBookSnapshot.Asks.TryRemove(order.Id, out var _);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderEventType), orderEventType, null);
            }
        }

        protected string ConvertSymbolFromLykkeToExchange(string symbol)
        {
            if (!CurrencyMappingProvider.CurrencyMapping.TryGetValue(symbol, out var result))
            {
                throw new ArgumentException($"Symbol {symbol} is not mapped to BitMex value");
            }
            return result;
        }

        protected Instrument ConvertSymbolFromExchangeToLykke(string symbol)
        {
            var result = CurrencyMappingProvider.CurrencyMapping.FirstOrDefault(kv => kv.Value == symbol).Key;
            if (result == null)
            {
                throw new ArgumentException($"Symbol {symbol} is not mapped to lykke value");
            }
            return new Instrument(ExchangeName, result);
        }

        private static void AddOrUpdateOrders(OrderBookSnapshot orderBookSnapshot,
            IEnumerable<OrderBookItem> newOrders)
        {
            foreach (var order in newOrders)
            {
                if (order.IsBuy)
                    orderBookSnapshot.Bids[order.Id] = order;
                else
                    orderBookSnapshot.Asks[order.Id] = order;
            }
        }


        private bool NeedThrottle()
        {
            var result = false;
            if (MaxOrderBookRate == 0)
            {
                return true;
            }
            if (_currentPublicationsNum >= MaxOrderBookRate)
            {
                var now = DateTime.UtcNow;
                if ((now - _lastPublishTime).TotalSeconds > 1)
                {
                    _currentPublicationsNum = 0;
                    _lastPublishTime = now;
                }
                else
                {
                    result = true;
                }
            }
            _currentPublicationsNum++;
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
                _cancellationTokenSource?.Dispose();
            }
        }
    }
}
