// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace TradingBot.Exchanges.Concrete.AutorestClient.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Persistent API Keys for Developers
    /// </summary>
    public partial class APIKey
    {
        /// <summary>
        /// Initializes a new instance of the APIKey class.
        /// </summary>
        public APIKey()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the APIKey class.
        /// </summary>
        public APIKey(string id, string secret, string name, double nonce, double userId, string cidr = default(string), IList<object> permissions = default(IList<object>), bool? enabled = default(bool?), System.DateTime? created = default(System.DateTime?))
        {
            Id = id;
            Secret = secret;
            Name = name;
            Nonce = nonce;
            Cidr = cidr;
            Permissions = permissions;
            Enabled = enabled;
            UserId = userId;
            Created = created;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "nonce")]
        public double Nonce { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "cidr")]
        public string Cidr { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "permissions")]
        public IList<object> Permissions { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "enabled")]
        public bool? Enabled { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "userId")]
        public double UserId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public System.DateTime? Created { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Id == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Id");
            }
            if (Secret == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Secret");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (Id != null)
            {
                if (Id.Length > 24)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Id", 24);
                }
            }
            if (Secret != null)
            {
                if (Secret.Length > 48)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Secret", 48);
                }
            }
            if (Name != null)
            {
                if (Name.Length > 64)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Name", 64);
                }
            }
            if (Cidr != null)
            {
                if (Cidr.Length > 18)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Cidr", 18);
                }
            }
        }
    }
}
