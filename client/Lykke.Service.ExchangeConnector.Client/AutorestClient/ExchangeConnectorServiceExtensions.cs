// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.ExchangeConnector.Client
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for ExchangeConnectorService.
    /// </summary>
    public static partial class ExchangeConnectorServiceExtensions
    {
            /// <summary>
            /// Returns full balance information on the exchange
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='exchangeName'>
            /// The exchange name
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<TradeBalanceModel>> GetTradeBalanceAsync(this IExchangeConnectorService operations, string exchangeName = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetTradeBalanceWithHttpMessagesAsync(exchangeName, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get a list of all connected exchanges
            /// </summary>
            /// <remarks>
            /// The names of available exchanges participates in API calls for
            /// exchange-specific methods
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<string>> GetSupportedExchangesAsync(this IExchangeConnectorService operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetSupportedExchangesWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get information about a specific exchange
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='exchangeName'>
            /// Name of the specific exchange
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ExchangeInformationModel> GetExchangeInfoAsync(this IExchangeConnectorService operations, string exchangeName, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetExchangeInfoWithHttpMessagesAsync(exchangeName, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Returns ratings of exchanges
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetRatingAsync(this IExchangeConnectorService operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetRatingWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Checks service is alive
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IsAliveResponseModel> IsAliveAsync(this IExchangeConnectorService operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.IsAliveWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Returns information about the earlier placed order
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// The order id
            /// </param>
            /// <param name='exchangeName'>
            /// The exchange name
            /// </param>
            /// <param name='instrument'>
            /// The instrument name of the order
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetOrderAsync(this IExchangeConnectorService operations, string id, string exchangeName = default(string), string instrument = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetOrderWithHttpMessagesAsync(id, exchangeName, instrument, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Cancels the existing order
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// The order id to cancel
            /// </param>
            /// <param name='exchangeName'>
            /// The exchange name
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> CancelOrderAsync(this IExchangeConnectorService operations, string id, string exchangeName = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CancelOrderWithHttpMessagesAsync(id, exchangeName, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Places a new order on the exchange
            /// &lt;param name="orderModel"&gt;A new order&lt;/param&gt;
            /// </summary>
            /// <remarks>
            /// In the location header of successful response placed an URL for getting
            /// info about the order
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='orderModel'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> CreateOrderAsync(this IExchangeConnectorService operations, OrderModel orderModel = default(OrderModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateOrderWithHttpMessagesAsync(orderModel, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Returns information about opened positions
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='exchangeName'>
            /// The exchange name
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetOpenedPositionAsync(this IExchangeConnectorService operations, string exchangeName = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetOpenedPositionWithHttpMessagesAsync(exchangeName, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
