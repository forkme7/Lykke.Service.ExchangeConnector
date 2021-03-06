// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

using Microsoft.Rest;
using Newtonsoft.Json;

namespace Lykke.ExternalExchangesApi.Exchanges.BitMex.AutorestClient.Models
{
    public partial class Margin
    {
        /// <summary>
        /// Initializes a new instance of the Margin class.
        /// </summary>
        public Margin()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Margin class.
        /// </summary>
        public Margin(double account, string currency, double? riskLimit = default(double?), string prevState = default(string), string state = default(string), string action = default(string), double? amount = default(double?), double? pendingCredit = default(double?), double? pendingDebit = default(double?), double? confirmedDebit = default(double?), double? prevRealisedPnl = default(double?), double? prevUnrealisedPnl = default(double?), double? grossComm = default(double?), double? grossOpenCost = default(double?), double? grossOpenPremium = default(double?), double? grossExecCost = default(double?), double? grossMarkValue = default(double?), double? riskValue = default(double?), double? taxableMargin = default(double?), double? initMargin = default(double?), double? maintMargin = default(double?), double? sessionMargin = default(double?), double? targetExcessMargin = default(double?), double? varMargin = default(double?), double? realisedPnl = default(double?), double? unrealisedPnl = default(double?), double? indicativeTax = default(double?), double? unrealisedProfit = default(double?), double? syntheticMargin = default(double?), double? walletBalance = default(double?), double? marginBalance = default(double?), double? marginBalancePcnt = default(double?), double? marginLeverage = default(double?), double? marginUsedPcnt = default(double?), double? excessMargin = default(double?), double? excessMarginPcnt = default(double?), double? availableMargin = default(double?), double? withdrawableMargin = default(double?), System.DateTime? timestamp = default(System.DateTime?), double? grossLastValue = default(double?), double? commission = default(double?))
        {
            Account = account;
            Currency = currency;
            RiskLimit = riskLimit;
            PrevState = prevState;
            State = state;
            Action = action;
            Amount = amount;
            PendingCredit = pendingCredit;
            PendingDebit = pendingDebit;
            ConfirmedDebit = confirmedDebit;
            PrevRealisedPnl = prevRealisedPnl;
            PrevUnrealisedPnl = prevUnrealisedPnl;
            GrossComm = grossComm;
            GrossOpenCost = grossOpenCost;
            GrossOpenPremium = grossOpenPremium;
            GrossExecCost = grossExecCost;
            GrossMarkValue = grossMarkValue;
            RiskValue = riskValue;
            TaxableMargin = taxableMargin;
            InitMargin = initMargin;
            MaintMargin = maintMargin;
            SessionMargin = sessionMargin;
            TargetExcessMargin = targetExcessMargin;
            VarMargin = varMargin;
            RealisedPnl = realisedPnl;
            UnrealisedPnl = unrealisedPnl;
            IndicativeTax = indicativeTax;
            UnrealisedProfit = unrealisedProfit;
            SyntheticMargin = syntheticMargin;
            WalletBalance = walletBalance;
            MarginBalance = marginBalance;
            MarginBalancePcnt = marginBalancePcnt;
            MarginLeverage = marginLeverage;
            MarginUsedPcnt = marginUsedPcnt;
            ExcessMargin = excessMargin;
            ExcessMarginPcnt = excessMarginPcnt;
            AvailableMargin = availableMargin;
            WithdrawableMargin = withdrawableMargin;
            Timestamp = timestamp;
            GrossLastValue = grossLastValue;
            Commission = commission;
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
        [JsonProperty(PropertyName = "riskLimit")]
        public double? RiskLimit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "prevState")]
        public string PrevState { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

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
        [JsonProperty(PropertyName = "prevRealisedPnl")]
        public double? PrevRealisedPnl { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "prevUnrealisedPnl")]
        public double? PrevUnrealisedPnl { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "grossComm")]
        public double? GrossComm { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "grossOpenCost")]
        public double? GrossOpenCost { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "grossOpenPremium")]
        public double? GrossOpenPremium { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "grossExecCost")]
        public double? GrossExecCost { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "grossMarkValue")]
        public double? GrossMarkValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "riskValue")]
        public double? RiskValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "taxableMargin")]
        public double? TaxableMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "initMargin")]
        public double? InitMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "maintMargin")]
        public double? MaintMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "sessionMargin")]
        public double? SessionMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "targetExcessMargin")]
        public double? TargetExcessMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "varMargin")]
        public double? VarMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "realisedPnl")]
        public double? RealisedPnl { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "unrealisedPnl")]
        public double? UnrealisedPnl { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "indicativeTax")]
        public double? IndicativeTax { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "unrealisedProfit")]
        public double? UnrealisedProfit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "syntheticMargin")]
        public double? SyntheticMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "walletBalance")]
        public double? WalletBalance { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "marginBalance")]
        public double? MarginBalance { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "marginBalancePcnt")]
        public double? MarginBalancePcnt { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "marginLeverage")]
        public double? MarginLeverage { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "marginUsedPcnt")]
        public double? MarginUsedPcnt { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "excessMargin")]
        public double? ExcessMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "excessMarginPcnt")]
        public double? ExcessMarginPcnt { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "availableMargin")]
        public double? AvailableMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "withdrawableMargin")]
        public double? WithdrawableMargin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public System.DateTime? Timestamp { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "grossLastValue")]
        public double? GrossLastValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "commission")]
        public double? Commission { get; set; }

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
