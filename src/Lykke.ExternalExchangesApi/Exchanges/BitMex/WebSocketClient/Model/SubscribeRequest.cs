﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Lykke.ExternalExchangesApi.Exchanges.BitMex.WebSocketClient.Model
{
    public class SubscribeRequest
    {
        [JsonProperty("op")]
        public string Operation { get; set; }

        [JsonProperty("args")]
        public IReadOnlyCollection<string> Arguments { get; set; }

        public static SubscribeRequest BuildRequest(params Tuple<string, string>[] filter)
        {
            return new SubscribeRequest
            {
                Operation = "subscribe",
                Arguments = filter.Select(f => $"{f.Item1}:{f.Item2}").ToArray()
            };
        }
    }
}
