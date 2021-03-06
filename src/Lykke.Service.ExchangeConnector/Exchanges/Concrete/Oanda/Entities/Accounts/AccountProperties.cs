﻿using System.Collections.Generic;

namespace TradingBot.Exchanges.Concrete.Oanda.Entities.Accounts
{
    public class AccountProperties
    {
        public string Id { get; set; }

        public int Mt4AccountId { get; set; }

        public List<string> Tags { get; set; }
    }
}
