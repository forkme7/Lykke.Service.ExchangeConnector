﻿using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.ExternalExchangesApi.Exchanges.Abstractions;
using TradingBot.Exchanges.Concrete.Oanda;
using TradingBot.Exchanges.Concrete.Oanda.Endpoints;
using Xunit;

namespace TradingBot.Tests.OandaApiTests
{
    public class AccountsApiTests
    {
        private string GetToken => OandaAuth.Token;
        private string accountId => "";

        private Accounts CreateAccountsApi()
        {
            return new Accounts(new ApiClient(OandaHttpClient.CreateHttpClient(GetToken), new LogToConsole()));
        }

        [Fact]
        public async Task GetAccountsDetails()
        {
            var accounts = CreateAccountsApi();

            var result = await accounts.GetAccounts();

            Assert.Single(result.Accounts);
        }

        [Fact]
        public async Task GetAccountDetails()
        {
            var api = CreateAccountsApi();

            var result = await api.GetAccountDetails(accountId);
        }

        [Fact]
        public async Task GetAcccountInstruments()
        {
            var api = CreateAccountsApi();

            var result = await api.GetAccountInstruments(accountId);

            System.IO.File.WriteAllLines("onanda_instruments.csv", result.Instruments.Select(x => x.DisplayName));
        }
    }
}
