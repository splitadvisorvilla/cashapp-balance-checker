using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CashAppBalanceChecker
{
    /// <summary>
    /// Main entry point for the CashApp balance checker application.
    /// </summary>
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Starting CashApp Balance Checker...");

            // Configuration: In a real app, these would come from secure config or env vars
            var cashAppToken = Environment.GetEnvironmentVariable("CASHAPP_API_TOKEN") ?? "demo_token_123";
            var cashAppClientId = Environment.GetEnvironmentVariable("CASHAPP_CLIENT_ID") ?? "demo_client_id";

            // Initialize the balance checker service
            var balanceService = new CashAppBalanceService(cashAppToken, cashAppClientId, logger);

            // Retrieve and display the balance
            try
            {
                var balance = await balanceService.GetBalanceAsync();
                Console.WriteLine($"Current CashApp Balance: ${balance:F2}");
                logger.LogInformation("Balance retrieved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to retrieve CashApp balance.");
                Console.WriteLine("Error: Unable to fetch balance. Check credentials and network.");
            }

            logger.LogInformation("CashApp Balance Checker finished.");
        }
    }
}
