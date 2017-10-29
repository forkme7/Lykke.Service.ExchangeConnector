// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace TradingBot.Exchanges.Concrete.AutorestClient.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Insurance Fund Data
    /// </summary>
    public partial class Insurance
    {
        /// <summary>
        /// Initializes a new instance of the Insurance class.
        /// </summary>
        public Insurance()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Insurance class.
        /// </summary>
        public Insurance(string currency, System.DateTime timestamp, double? walletBalance = default(double?))
        {
            Currency = currency;
            Timestamp = timestamp;
            WalletBalance = walletBalance;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public System.DateTime Timestamp { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "walletBalance")]
        public double? WalletBalance { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Currency == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Currency");
            }
        }
    }
}
