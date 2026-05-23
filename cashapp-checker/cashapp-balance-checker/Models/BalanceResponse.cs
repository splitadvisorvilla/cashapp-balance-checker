using Newtonsoft.Json;

namespace CashAppBalanceChecker.Models
{
    /// <summary>
    /// Represents the response from the CashApp API balance endpoint.
    /// </summary>
    public class BalanceResponse
    {
        /// <summary>
        /// Gets or sets the current balance amount.
        /// </summary>
        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the currency code (e.g., "USD").
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp of the balance retrieval.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;
    }
}
