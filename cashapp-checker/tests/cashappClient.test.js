/**
 * Unit tests for CashAppClient module.
 * 
 * Tests API interaction with mocked axios to avoid real network calls.
 */

const CashAppClient = require('../src/cashappClient');
const axios = require('axios');

// Mock axios module
jest.mock('axios');

describe('CashAppClient', () => {
  let client;
  const mockCashtag = '$testuser';
  const mockToken = 'test-token-123';

  beforeEach(() => {
    // Clear all mocks before each test
    jest.clearAllMocks();
    client = new CashAppClient(mockCashtag, mockToken, { verbose: false });
  });

  describe('getBalance', () => {
    test('should return balance object on successful API call', async () => {
      const mockResponse = {
        data: {
          balance: 250.75,
          currency: 'USD',
        },
      };
      axios.create.mockReturnValue({
        get: jest.fn().mockResolvedValue(mockResponse),
      });

      // Re-initialize client with mocked axios
      client = new CashAppClient(mockCashtag, mockToken);

      const balance = await client.getBalance();
      expect(balance).toEqual({ amount: 250.75, currency: 'USD' });
    });

    test('should throw error when API returns invalid data', async () => {
      const mockResponse = {
        data: {},
      };
      axios.create.mockReturnValue({
        get: jest.fn().mockResolvedValue(mockResponse),
      });

      client = new CashAppClient(mockCashtag, mockToken);

      await expect(client.getBalance()).rejects.toThrow('Invalid response format from CashApp API');
    });

    test('should throw error on network failure', async () => {
      axios.create.mockReturnValue({
        get: jest.fn().mockRejectedValue(new Error('Network error')),
      });

      client = new CashAppClient(mockCashtag, mockToken);

      await expect(client.getBalance()).rejects.toThrow('Network error: Unable to reach CashApp API');
    });

    test('should throw error on server error response', async () => {
      const errorResponse = {
        response: {
          status: 401,
          data: { message: 'Unauthorized' },
        },
      };
      axios.create.mockReturnValue({
        get: jest.fn().mockRejectedValue(errorResponse),
      });

      client = new CashAppClient(mockCashtag, mockToken);

      await expect(client.getBalance()).rejects.toThrow('API error: 401 - Unauthorized');
    });
  });
});
