// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace TradingBot.Exchanges.Concrete.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class APIKeyremoveOKResponse
    {
        /// <summary>
        /// Initializes a new instance of the APIKeyremoveOKResponse class.
        /// </summary>
        public APIKeyremoveOKResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the APIKeyremoveOKResponse class.
        /// </summary>
        public APIKeyremoveOKResponse(bool? success = default(bool?))
        {
            Success = success;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public bool? Success { get; set; }

    }
}
