using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CashAppBalanceChecker.Helpers
{
    /// <summary>
    /// Helper class for common API operations.
    /// </summary>
    public static class ApiHelper
    {
        /// <summary>
        /// Creates an HTTP request message with the specified method, URL, and optional body.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="url">The request URL.</param>
        /// <param name="body">Optional JSON-serializable body object.</param>
        /// <returns>An HttpRequestMessage instance.</returns>
        public static HttpRequestMessage CreateRequest(HttpMethod method, string url, object? body = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (body != null)
            {
                var jsonBody = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }
            return request;
        }

        /// <summary>
        /// Sends a request and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="T">The response type.</typeparam>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="request">The request to send.</param>
        /// <returns>The deserialized response object.</returns>
        /// <exception cref="HttpRequestException">Thrown when the response is not successful.</exception>
        public static async Task<T> SendAndDeserializeAsync<T>(HttpClient client, HttpRequestMessage request)
        {
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed: {response.StatusCode} - {errorBody}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString) 
                ?? throw new InvalidOperationException("Response deserialization returned null.");
        }

        /// <summary>
        /// Validates that a string is not null or empty, throwing an exception if invalid.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <param name="paramName">The parameter name for the exception.</param>
        /// <exception cref="ArgumentException">Thrown if value is null or empty.</exception>
        public static void ValidateNonEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
            }
        }
    }
}
