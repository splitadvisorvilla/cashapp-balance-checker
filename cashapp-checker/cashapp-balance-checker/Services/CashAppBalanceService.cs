using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CashAppBalanceChecker
{
    /// <summary>
    /// Service responsible for interacting with the CashApp API to retrieve account balance.
    /// </summary>
    public class CashAppBalanceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken;
        private readonly string _clientId;
        private readonly ILogger _logger;

        private const string BaseUrl = "https://api.cash.app/v1";

        /// <summary>
        /// Initializes a new instance of the <see cref="CashAppBalanceService"/> class.
        /// </summary>
        /// <param name="apiToken">The API token for authentication.</param>
        /// <param name="clientId">The client ID for the CashApp application.</param>
        /// <param name="logger">Logger instance for logging.</param>
        public CashAppBalanceService(string apiToken, string clientId, ILogger logger)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiToken}");
            _httpClient.DefaultRequestHeaders.Add("X-Client-Id", _clientId);
        }

        /// <summary>
        /// Retrieves the current CashApp balance asynchronously.
        /// </summary>
        /// <returns>The balance amount as a decimal.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the response format is invalid.</exception>
        public async Task<decimal> GetBalanceAsync()
        {
            _logger.LogInformation("Fetching CashApp balance from API...");

            var requestUrl = $"{BaseUrl}/account/balance";
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(requestUrl);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while contacting CashApp API.");
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("API returned {StatusCode}: {ErrorBody}", response.StatusCode, errorBody);
                throw new HttpRequestException($"API request failed with status {response.StatusCode}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Raw API response: {Response}", jsonString);

            // Parse JSON response. Expected format: { "balance": 123.45, "currency": "USD" }
            JObject jsonObject;
            try
            {
                jsonObject = JObject.Parse(jsonString);
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse API response JSON.");
                throw new InvalidOperationException("Invalid response format from CashApp API.", ex);
            }

            var balanceToken = jsonObject["balance"];
            if (balanceToken == null)
            {
                _logger.LogError("Response JSON missing 'balance' field.");
                throw new InvalidOperationException("API response missing balance data.");
            }

            decimal balance = balanceToken.Value<decimal>();
            _logger.LogInformation("Successfully retrieved balance: {Balance}", balance);

            return balance;
        }

        /// <summary>
        /// Disposes the underlying HttpClient.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
