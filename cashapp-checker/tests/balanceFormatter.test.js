/**
 * Unit tests for BalanceFormatter module.
 * 
 * Tests formatting logic with various currency and amount scenarios.
 */

const BalanceFormatter = require('../src/balanceFormatter');

describe('BalanceFormatter', () => {
  describe('format', () => {
    test('should format USD balance correctly', () => {
      const balance = { amount: 150.5, currency: 'USD' };
      const result = BalanceFormatter.format(balance);
      expect(result).toBe('$150.50 USD');
    });

    test('should format EUR balance with symbol', () => {
      const balance = { amount: 89.99, currency: 'EUR' };
      const result = BalanceFormatter.format(balance);
      expect(result).toBe('€89.99 EUR');
    });

    test('should format zero balance', () => {
      const balance = { amount: 0, currency: 'USD' };
      const result = BalanceFormatter.format(balance);
      expect(result).toBe('$0.00 USD');
    });

    test('should format large numbers with commas (via toFixed)', () => {
      const balance = { amount: 1234567.89, currency: 'USD' };
      const result = BalanceFormatter.format(balance);
      expect(result).toBe('$1234567.89 USD');
    });

    test('should throw error for non-numeric amount', () => {
      const balance = { amount: 'abc', currency: 'USD' };
      expect(() => BalanceFormatter.format(balance)).toThrow('Invalid balance amount: must be a number');
    });

    test('should throw error for NaN amount', () => {
      const balance = { amount: NaN, currency: 'USD' };
      expect(() => BalanceFormatter.format(balance)).toThrow('Invalid balance amount: must be a number');
    });

    test('should use currency code as fallback for unknown currencies', () => {
      const balance = { amount: 100, currency: 'XYZ' };
      const result = BalanceFormatter.format(balance);
      expect(result).toBe('XYZ 100.00 XYZ');
    });
  });

  describe('getCurrencySymbol', () => {
    test('should return $ for USD', () => {
      expect(BalanceFormatter.getCurrencySymbol('USD')).toBe('$');
    });

    test('should return € for EUR', () => {
      expect(BalanceFormatter.getCurrencySymbol('EUR')).toBe('€');
    });

    test('should return currency code for unknown', () => {
      expect(BalanceFormatter.getCurrencySymbol('BTC')).toBe('BTC ');
    });
  });
});
