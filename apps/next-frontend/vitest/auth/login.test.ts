import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { POST as login } from '@/app/api/auth/login/route';

describe('Login', () => {
  const gatewayUrl = 'http://localhost:8080';
  const request = {
    username: 'user@example.com',
    password: 'correct-password',
  };

  beforeEach(() => {
    process.env.GATEWAY_URL = gatewayUrl;
    vi.stubGlobal('fetch', vi.fn());
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
    delete process.env.GATEWAY_URL;
  });

  it('sends the login request with credentials enabled for the returned cookie', async () => {
    const response = { isSuccess: true, errorMessage: '' };
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify(response), { status: 200 }),
    );

    const routeRequest = new Request('http://localhost/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    await expect(login(routeRequest)).resolves.toHaveProperty('status', 200);

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

    const routeRequest = new Request('http://localhost/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    const result = await login(routeRequest);

    await expect(result.json()).resolves.toEqual(response);
  });

  it('returns a fallback error when the request fails', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined);
    vi.mocked(fetch).mockRejectedValue(new Error('Network error'));

    const routeRequest = new Request('http://localhost/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    const result = await login(routeRequest);

    await expect(result.json()).resolves.toEqual({
      isSuccess: false,
      errorMessage: 'An error occurred while logging in.',
    });
    expect(result.status).toBe(500);
  });
});
