import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { Login } from '../../api/auth/login';

describe('Login', () => {
  const gatewayUrl = 'http://localhost:8080';
  const request = {
    username: 'user@example.com',
    password: 'correct-password',
  };

  beforeEach(() => {
    process.env.NEXT_PUBLIC_GATEWAY_URL = gatewayUrl;
    vi.stubGlobal('fetch', vi.fn());
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
    delete process.env.NEXT_PUBLIC_GATEWAY_URL;
  });

  it('sends the login request with credentials enabled for the returned cookie', async () => {
    const response = { isSuccess: true, errorMessage: '' };
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify(response), { status: 200 }),
    );

    await expect(Login(request)).resolves.toEqual(response);

    expect(fetch).toHaveBeenCalledWith(`${gatewayUrl}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
      credentials: 'include',
    });
  });

  it('returns the API error response when login is unsuccessful', async () => {
    const response = {
      isSuccess: false,
      errorMessage: 'Invalid credentials',
    };
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify(response), { status: 401 }),
    );

    await expect(Login(request)).resolves.toEqual(response);
  });

  it('returns a fallback error when the request fails', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined);
    vi.mocked(fetch).mockRejectedValue(new Error('Network error'));

    await expect(Login(request)).resolves.toEqual({
      isSuccess: false,
      errorMessage: 'An error occurred while logging in.',
    });
  });
});
