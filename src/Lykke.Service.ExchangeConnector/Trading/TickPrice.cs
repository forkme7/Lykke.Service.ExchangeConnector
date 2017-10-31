﻿using System;
using Newtonsoft.Json;

namespace TradingBot.Trading
{
	public class TickPrice
	{
        public TickPrice(Instrument instrument, DateTime time, decimal mid)
		{
		    Instrument = instrument;
		    
			Time = time;
			Ask = mid;
		    Bid = mid;
			Mid = mid;
		}

		[JsonConstructor]
		public TickPrice(Instrument instrument, DateTime time, decimal ask, decimal bid)
		{
		    Instrument = instrument;
		    
			Time = time;
			Ask = ask;
			Bid = bid;
		    
		    Mid = (ask + bid) / 2m;
		}

		public TickPrice(Instrument instrument, DateTime time, decimal ask, decimal bid, decimal mid)
		{
		    Instrument = instrument;
		    
			Time = time;
			Ask = ask;
			Bid = bid;
			Mid = mid;
		}

	    public Instrument Instrument { get; }
	    
		public DateTime Time { get; }

		public decimal Ask { get; }

		public decimal Bid { get; }

		[JsonIgnore]
		public decimal Mid { get; }

		public override string ToString()
		{
			return $"[TickPrice: Time={Time}, Ask={Ask:C}, Bid={Bid:C}, Mid={Mid:C}]";
		}
	}
}
