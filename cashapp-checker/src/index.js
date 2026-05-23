/**
 * CashApp Balance Checker - Main Entry Point
 * 
 * This script checks the balance of a CashApp account using a simulated API.
 * It reads credentials from environment variables and outputs the balance.
 * 
 * Usage: node src/index.js [--verbose]
 */

require('dotenv').config();
const CashAppClient = require('./cashappClient');
const BalanceFormatter = require('./balanceFormatter');

/**
 * Main function to orchestrate balance checking.
 */
async function main() {
  const verbose = process.argv.includes('--verbose');

  // Validate required environment variables
  const cashtag = process.env.CASHTAG;
  const authToken = process.env.AUTH_TOKEN;

  if (!cashtag || !authToken) {
    console.error('Error: Missing required environment variables.');
    console.error('Please set CASHTAG and AUTH_TOKEN in your .env file.');
    process.exit(1);
  }

  try {
    const client = new CashAppClient(cashtag, authToken, { verbose });
    const balance = await client.getBalance();
    const formatted = BalanceFormatter.format(balance);
    console.log(`Balance for ${cashtag}: ${formatted}`);
  } catch (error) {
    console.error('Failed to retrieve balance:', error.message);
    process.exit(1);
  }
}

main();
