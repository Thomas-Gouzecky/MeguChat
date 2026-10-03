import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { POST as logout } from '@/app/api/auth/logout/route';

describe('POST /api/auth/logout', () => {
  const gatewayUrl = 'http://localhost:8080';

  beforeEach(() => {
    process.env.GATEWAY_URL = gatewayUrl;
    vi.stubGlobal('fetch', vi.fn());
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
    delete process.env.GATEWAY_URL;
  });

  it('forwards the browser cookie to the gateway', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify({ isSuccess: true }), { status: 200 }),
    );

    const request = new Request('http://localhost/api/auth/logout', {
      method: 'POST',
      headers: { Cookie: 'auth=session-123' },
    });

    const response = await logout(request);

    expect(response.status).toBe(200);
    expect(fetch).toHaveBeenCalledWith(`${gatewayUrl}/api/auth/logout`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Cookie: 'auth=session-123',
      },
    });
  });

  it('forwards the gateway response status and headers', async () => {
    const gatewayResponse = new Response(null, {
      status: 401,
      headers: { 'Set-Cookie': 'auth=; Max-Age=0; Path=/' },
    });
    vi.mocked(fetch).mockResolvedValue(gatewayResponse);

    const response = await logout(
      new Request('http://localhost/api/auth/logout', { method: 'POST' }),
    );

    expect(response.status).toBe(401);
    expect(response.headers.get('Set-Cookie')).toBe('auth=; Max-Age=0; Path=/');
  });

  it('returns a 500 response when the gateway request fails', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined);
    vi.mocked(fetch).mockRejectedValue(new Error('Gateway unavailable'));

    const response = await logout(
      new Request('http://localhost/api/auth/logout', { method: 'POST' }),
    );

    expect(response.status).toBe(500);
    await expect(response.json()).resolves.toEqual({
      isSuccess: false,
      errorMessage: 'An error occurred while logging out.',
    });
  });
});
