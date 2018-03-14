﻿using System.Collections.Generic;

namespace TradingBot.Infrastructure.Configuration
{
    public sealed class GdaxExchangeConfiguration : IExchangeConfiguration
    {
        public GdaxExchangeConfiguration()
        {
            UseSupportedCurrencySymbolsAsFilter = true;
        }

        public double InitialRating { get; set; }

        [Lykke.SettingsReader.Attributes.Optional]
        public bool? UseSupportedCurrencySymbolsAsFilter { get; set; }

        public bool Enabled { get; set; }

        public bool SaveOrderBooksToAzure { get; set; }

        public bool PubQuotesToRabbit { get; set; }

        public string PassPhrase { get; set; }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public string RestEndpointUrl { get; set; }

        public string WssEndpointUrl { get; set; }

        public string UserAgent { get; set; }

        public IReadOnlyCollection<CurrencySymbol> SupportedCurrencySymbols { get; set; }
    }
}
