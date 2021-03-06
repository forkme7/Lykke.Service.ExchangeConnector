﻿using Newtonsoft.Json;

namespace Lykke.ExternalExchangesApi.Exchanges.GDAX.RestClient.Entities
{
    public sealed class GdaxActiveOrdersPost : GdaxPostContentBase
    {
        /// <summary>
        /// This class can be used to send a cancel message in addition to 
        /// retrieving the current status of an order.
        /// </summary>
        [JsonProperty("order_id")]
        public long OrderId { get; set; }
    }
}
