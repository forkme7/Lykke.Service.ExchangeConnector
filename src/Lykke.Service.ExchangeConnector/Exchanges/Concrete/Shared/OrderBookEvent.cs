﻿using System;
using System.Collections.Generic;

namespace TradingBot.Exchanges.Concrete.Shared
{
    internal class OrderBookEvent
    {
        public string SnapshotId { get; set; }

        public DateTime InternalTimestamp { get; }

        public DateTime OrderEventTimestamp { get; set; }

        public OrderBookEventType EventType { get; set; }

        public IReadOnlyCollection<OrderBookItem> OrderItems { get; set; }

        public OrderBookEvent()
        {
            InternalTimestamp = DateTime.UtcNow;
        }
    }
}
