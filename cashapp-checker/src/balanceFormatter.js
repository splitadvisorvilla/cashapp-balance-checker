/**
 * BalanceFormatter - Utility to format balance data for display.
 * 
 * Provides methods to convert raw balance numbers into human-readable strings.
 */

class BalanceFormatter {
  /**
   * Format a balance object into a readable string.
   * @param {Object} balance - The balance object with amount and currency.
   * @param {number} balance.amount - The numeric balance value.
   * @param {string} balance.currency - The currency code (e.g., USD).
   * @returns {string} Formatted balance string (e.g., "$150.00 USD").
   */
  static format(balance) {
    const { amount, currency } = balance;

    // Validate input
    if (typeof amount !== 'number' || isNaN(amount)) {
      throw new Error('Invalid balance amount: must be a number');
    }

    // Determine currency symbol
    const symbol = this.getCurrencySymbol(currency);

    // Format with two decimal places
    const formattedAmount = amount.toFixed(2);

    return `${symbol}${formattedAmount} ${currency}`;
  }

  /**
   * Get the currency symbol for a given currency code.
   * @param {string} currencyCode - ISO 4217 currency code.
   * @returns {string} Currency symbol (e.g., $, €, £).
   */
  static getCurrencySymbol(currencyCode) {
    const symbols = {
      'USD': '$',
      'EUR': '€',
      'GBP': '£',
      'JPY': '¥',
      'CAD': 'C$',
      'AUD': 'A$',
    };

    return symbols[currencyCode] || currencyCode + ' ';
  }
}

module.exports = BalanceFormatter;
