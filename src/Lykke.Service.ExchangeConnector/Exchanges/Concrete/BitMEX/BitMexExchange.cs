﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Newtonsoft.Json;
using TradingBot.Communications;
using TradingBot.Exchanges.Abstractions;
using Lykke.ExternalExchangesApi.Exceptions;
using Lykke.ExternalExchangesApi.Exchanges.BitMex.AutorestClient;
using Lykke.ExternalExchangesApi.Exchanges.BitMex.AutorestClient.Models;
using TradingBot.Infrastructure.Configuration;
using TradingBot.Models.Api;
using TradingBot.Repositories;
using TradingBot.Trading;
using Instrument = TradingBot.Trading.Instrument;
using Order = Lykke.ExternalExchangesApi.Exchanges.BitMex.AutorestClient.Models.Order;
using Position = Lykke.ExternalExchangesApi.Exchanges.BitMex.AutorestClient.Models.Position;

namespace TradingBot.Exchanges.Concrete.BitMEX
{
    internal class BitMexExchange : Exchange
    {
        private readonly BitMexOrderBooksHarvester _orderBooksHarvester;
        private readonly BitMexOrderHarvester _orderHarvester;
        private readonly BitMexPriceHarvester _priceHarvester;
        private readonly BitMexExecutionHarvester _executionHarvester;
        private readonly IBitMEXAPI _exchangeApi;
        public new const string Name = "bitmex";

        public BitMexExchange(BitMexExchangeConfiguration configuration,
            TranslatedSignalsRepository translatedSignalsRepository,
            BitMexOrderBooksHarvester orderBooksHarvester,
            BitMexOrderHarvester orderHarvester,
            BitMexPriceHarvester priceHarvester,
            BitMexExecutionHarvester executionHarvester,
            ILog log)
            : base(Name, configuration, translatedSignalsRepository, log)
        {
            _orderBooksHarvester = orderBooksHarvester;
            _orderHarvester = orderHarvester;
            _priceHarvester = priceHarvester;
            _executionHarvester = executionHarvester;


            var credenitals = new BitMexServiceClientCredentials(configuration.ApiKey, configuration.ApiSecret);
            _exchangeApi = new BitMEXAPI(credenitals)
            {
                BaseUri = new Uri(configuration.EndpointUrl)
            };

            orderBooksHarvester.MaxOrderBookRate = configuration.MaxOrderBookRate;
        }

        public override async Task<ExecutionReport> AddOrderAndWaitExecution(TradingSignal signal, TranslatedSignalTableEntity translatedSignal, TimeSpan timeout)
        {
            //  var symbol = BitMexModelConverter.ConvertSymbolFromLykkeToBitMex(instrument.Name, _configuration);
            var symbol = "XBTUSD"; //HACK Hard code!
            var volume = BitMexModelConverter.ConvertVolume(signal.Volume);
            var orderType = BitMexModelConverter.ConvertOrderType(signal.OrderType);
            var side = BitMexModelConverter.ConvertTradeType(signal.TradeType);
            var price = (double?)signal.Price;
            var ct = new CancellationTokenSource(timeout);

            var response = await _exchangeApi.OrdernewAsync(symbol, orderQty: volume, price: price, clOrdID: signal.OrderId, ordType: orderType, side: side, cancellationToken: ct.Token);

            if (response is Error error)
            {
                throw new ApiException(error.ErrorProperty.Message);
            }

            var order = (Order)response;
            return BitMexModelConverter.OrderToTrade(order);
        }

        public override async Task<ExecutionReport> CancelOrderAndWaitExecution(TradingSignal signal, TranslatedSignalTableEntity translatedSignal, TimeSpan timeout)
        {
            var ct = new CancellationTokenSource(timeout);
            var id = signal.OrderId;
            var response = await _exchangeApi.OrdercancelAsync(cancellationToken: ct.Token, orderID: id);

            if (response is Error error)
            {
                throw new ApiException(error.ErrorProperty.Message);
            }
            var res = EnsureCorrectResponse(id, response);
            return BitMexModelConverter.OrderToTrade(res[0]);
        }

        public override async Task<ExecutionReport> GetOrder(string id, Instrument instrument, TimeSpan timeout)
        {
            var filterObj = new { orderID = id };
            var filterArg = JsonConvert.SerializeObject(filterObj);
            var cts = new CancellationTokenSource(timeout);
            var response = await _exchangeApi.OrdergetOrdersAsync(filter: filterArg, cancellationToken: cts.Token);
            var res = EnsureCorrectResponse(id, response);
            return BitMexModelConverter.OrderToTrade(res[0]);
        }

        public override async Task<IEnumerable<ExecutionReport>> GetOpenOrders(TimeSpan timeout)
        {
            var filterObj = new { ordStatus = "New" };
            var filterArg = JsonConvert.SerializeObject(filterObj);
            var response = await _exchangeApi.OrdergetOrdersAsync(filter: filterArg);
            if (response is Error error)
            {
                throw new ApiException(error.ErrorProperty.Message);
            }

            var trades = ((IReadOnlyCollection<Order>)response).Select(
                BitMexModelConverter.OrderToTrade);
            return trades;
        }

        public override async Task<IReadOnlyCollection<TradeBalanceModel>> GetTradeBalances(TimeSpan timeout)
        {
            var cts = new CancellationTokenSource(timeout);
            var response = await _exchangeApi.UsergetMarginWithHttpMessagesAsync(cancellationToken: cts.Token);
            var bitmexMargin = response.Body;

            var model = BitMexModelConverter.ExchangeBalanceToModel(bitmexMargin);
            return new[] { model };
        }

        public override async Task<IReadOnlyCollection<PositionModel>> GetPositions(TimeSpan timeout)
        {
            var cts = new CancellationTokenSource(timeout);
            var onlyOpensFileter = "{\"isOpen\":true}";
            var response = await _exchangeApi.PositiongetAsync(cancellationToken: cts.Token, filter: onlyOpensFileter);

            if (response is Error error)
            {
                throw new ApiException(error.ErrorProperty.Message);
            }

            var model = ((IReadOnlyCollection<Position>)response).Select(BitMexModelConverter.ExchangePositionToModel).ToArray();
            return model;
        }

        private static IReadOnlyList<Order> EnsureCorrectResponse(string id, object response)
        {
            if (response is Error error)
            {
                throw new ApiException(error.ErrorProperty.Message);
            }
            var res = (IReadOnlyList<Order>)response;
            if (res.Count != 1)
            {
                throw new InvalidOperationException($"Received {res.Count} orders. Expected exactly one with id {id}");
            }
            return res;
        }

        protected override void StartImpl()
        {
            _executionHarvester.Start();
            _priceHarvester.Start();
            _orderHarvester.Start();
            _orderBooksHarvester.Start();
            OnConnected();
        }

        protected override void StopImpl()
        {
            _executionHarvester.Stop();
            _priceHarvester.Stop();
            _orderHarvester.Stop();
            _orderBooksHarvester.Stop();
            OnStopped();
        }
    }
}
