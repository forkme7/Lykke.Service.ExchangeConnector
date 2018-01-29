﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lykke.ExternalExchangesApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using TradingBot.Models.Api;

namespace TradingBot.Controllers.Api
{
    public sealed class ExchangesController : BaseApiController
    {

        public ExchangesController(IApplicationFacade app)
            : base(app)
        {
        }

        /// <summary>
        /// Get a list of all connected exchanges
        /// </summary>
        /// <remarks>The names of available exchanges participates in API calls for exchange-specific methods</remarks>
        [HttpGet]
        [SwaggerOperation("GetSupportedExchanges")]
        public IEnumerable<string> List()
        {
            return Application.GetExchanges().Select(x => x.Name);
        }

        /// <summary>
        /// Get information about a specific exchange
        /// </summary>
        /// <param name="exchangeName">Name of the specific exchange</param>
        [SwaggerOperation("GetExchangeInfo")]
        [HttpGet("{exchangeName}")]
        public ExchangeInformationModel Index(string exchangeName)
        {
            var exchange = Application.GetExchange(exchangeName);

            if (exchange == null)
            {
                throw new StatusCodeException(HttpStatusCode.InternalServerError, $"There isn't connected exchange with the name of {exchangeName}. Try GET api/exchanges for the list of all connected exchanges.");
            }

            return new ExchangeInformationModel
            {
                Name = exchangeName,
                State = exchange.State,
                Instruments = exchange.Instruments,
                StreamingSupport = exchange.StreamingSupport
            };
        }
    }
}
