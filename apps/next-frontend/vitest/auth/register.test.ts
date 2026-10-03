import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { Register } from '../../api/auth/register';

describe('Register', () => {
  const gatewayUrl = 'http://localhost:8080';
  const request = {
    username: 'new-user@example.com',
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

  it('sends the registration request and returns the API response', async () => {
    const response = { isSuccess: true, errorMessage: '' };
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify(response), { status: 201 }),
    );

    await expect(Register(request)).resolves.toEqual(response);

    expect(fetch).toHaveBeenCalledWith(`${gatewayUrl}/api/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
  });

  it('returns the API error response when registration is unsuccessful', async () => {
    const response = {
      isSuccess: false,
      errorMessage: 'Username is already registered',
    };
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify(response), { status: 400 }),
    );

    await expect(Register(request)).resolves.toEqual(response);
  });

  it('returns a fallback error when the request fails', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined);
    vi.mocked(fetch).mockRejectedValue(new Error('Network error'));

    await expect(Register(request)).resolves.toEqual({
      isSuccess: false,
      errorMessage: 'An error occurred while registering.',
    });
  });
});
