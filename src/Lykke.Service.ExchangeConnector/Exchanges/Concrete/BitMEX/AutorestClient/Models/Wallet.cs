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

    public partial class Wallet
    {
        /// <summary>
        /// Initializes a new instance of the Wallet class.
        /// </summary>
        public Wallet()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Wallet class.
        /// </summary>
        public Wallet(double account, string currency, double? prevDeposited = default(double?), double? prevWithdrawn = default(double?), double? prevTransferIn = default(double?), double? prevTransferOut = default(double?), double? prevAmount = default(double?), System.DateTime? prevTimestamp = default(System.DateTime?), double? deltaDeposited = default(double?), double? deltaWithdrawn = default(double?), double? deltaTransferIn = default(double?), double? deltaTransferOut = default(double?), double? deltaAmount = default(double?), double? deposited = default(double?), double? withdrawn = default(double?), double? transferIn = default(double?), double? transferOut = default(double?), double? amount = default(double?), double? pendingCredit = default(double?), double? pendingDebit = default(double?), double? confirmedDebit = default(double?), System.DateTime? timestamp = default(System.DateTime?), string addr = default(string), string script = default(string), IList<string> withdrawalLock = default(IList<string>))
        {
            Account = account;
            Currency = currency;
            PrevDeposited = prevDeposited;
            PrevWithdrawn = prevWithdrawn;
            PrevTransferIn = prevTransferIn;
            PrevTransferOut = prevTransferOut;
            PrevAmount = prevAmount;
            PrevTimestamp = prevTimestamp;
            DeltaDeposited = deltaDeposited;
            DeltaWithdrawn = deltaWithdrawn;
            DeltaTransferIn = deltaTransferIn;
            DeltaTransferOut = deltaTransferOut;
            DeltaAmount = deltaAmount;
            Deposited = deposited;
            Withdrawn = withdrawn;
            TransferIn = transferIn;
            TransferOut = transferOut;
            Amount = amount;
            PendingCredit = pendingCredit;
            PendingDebit = pendingDebit;
            ConfirmedDebit = confirmedDebit;
            Timestamp = timestamp;
            Addr = addr;
            Script = script;
            WithdrawalLock = withdrawalLock;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "account")]
        public double Account { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "prevDeposited")]
        public double? PrevDeposited { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "prevWithdrawn")]
        public double? PrevWithdrawn { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "prevTransferIn")]
        public double? PrevTransferIn { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "prevTransferOut")]
        public double? PrevTransferOut { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "prevAmount")]
        public double? PrevAmount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "prevTimestamp")]
        public System.DateTime? PrevTimestamp { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "deltaDeposited")]
        public double? DeltaDeposited { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "deltaWithdrawn")]
        public double? DeltaWithdrawn { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "deltaTransferIn")]
        public double? DeltaTransferIn { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "deltaTransferOut")]
        public double? DeltaTransferOut { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "deltaAmount")]
        public double? DeltaAmount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "deposited")]
        public double? Deposited { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "withdrawn")]
        public double? Withdrawn { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "transferIn")]
        public double? TransferIn { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "transferOut")]
        public double? TransferOut { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public double? Amount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "pendingCredit")]
        public double? PendingCredit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "pendingDebit")]
        public double? PendingDebit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "confirmedDebit")]
        public double? ConfirmedDebit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public System.DateTime? Timestamp { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "addr")]
        public string Addr { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "script")]
        public string Script { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "withdrawalLock")]
        public IList<string> WithdrawalLock { get; set; }

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
