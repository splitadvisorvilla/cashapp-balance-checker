/**
 * CashAppClient - Handles communication with CashApp's API.
 * 
 * This module simulates a real CashApp API client for educational purposes.
 * In production, replace the base URL and endpoints with official CashApp API.
 */

const axios = require('axios');

class CashAppClient {
  /**
   * Create a new CashAppClient instance.
   * @param {string} cashtag - The user's Cashtag (e.g., $username).
   * @param {string} authToken - Authentication token for API access.
   * @param {Object} [options] - Optional configuration.
   * @param {boolean} [options.verbose] - Enable verbose logging.
   */
  constructor(cashtag, authToken, options = {}) {
    this.cashtag = cashtag;
    this.authToken = authToken;
    this.verbose = options.verbose || false;

    // Base URL for CashApp API (simulated)
    this.baseUrl = 'https://api.squareup.com/v2/cashapp';

    // Configure axios instance with default headers
    this.client = axios.create({
      baseURL: this.baseUrl,
      timeout: 10000,
      headers: {
        'Authorization': `Bearer ${this.authToken}`,
        'Content-Type': 'application/json',
      },
    });
  }

  /**
   * Fetch the current balance for the user's CashApp account.
   * @returns {Promise<Object>} Balance object with amount and currency.
   * @throws {Error} If the API request fails.
   */
  async getBalance() {
    try {
      if (this.verbose) {
        console.log(`[CashAppClient] Fetching balance for ${this.cashtag}...`);
      }

      // Simulated API endpoint - replace with real endpoint in production
      const response = await this.client.get('/balance', {
        params: { cashtag: this.cashtag },
      });

      if (this.verbose) {
        console.log('[CashAppClient] Response received:', response.data);
      }

      // Validate response structure
      if (!response.data || typeof response.data.balance !== 'number') {
        throw new Error('Invalid response format from CashApp API');
      }

      return {
        amount: response.data.balance,
        currency: response.data.currency || 'USD',
      };
    } catch (error) {
      if (error.response) {
        // Server responded with error status
        throw new Error(`API error: ${error.response.status} - ${error.response.data.message || 'Unknown error'}`);
      } else if (error.request) {
        // No response received
        throw new Error('Network error: Unable to reach CashApp API');
      } else {
        // Other errors
        throw error;
      }
    }
  }
}

module.exports = CashAppClient;
