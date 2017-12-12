// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.ExchangeConnector.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Checks service is alive response
    /// </summary>
    public partial class IsAliveResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the IsAliveResponseModel class.
        /// </summary>
        public IsAliveResponseModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the IsAliveResponseModel class.
        /// </summary>
        /// <param name="version">API version</param>
        /// <param name="env">Environment variables</param>
        public IsAliveResponseModel(string version = default(string), string env = default(string))
        {
            Version = version;
            Env = env;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets API version
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets environment variables
        /// </summary>
        [JsonProperty(PropertyName = "env")]
        public string Env { get; set; }

    }
}
