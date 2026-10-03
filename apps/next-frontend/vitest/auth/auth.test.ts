import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { getCurrentUser } from '@/lib/auth/auth';

describe('getCurrentUser', () => {
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

  it('returns the authenticated user for a successful response', async () => {
    const user = { id: 'user-1', email: 'user@example.com' };
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify(user), { status: 200 }),
    );

    await expect(getCurrentUser('session=abc')).resolves.toEqual(user);
  });

  it('calls the auth endpoint with the cookie and no-store cache policy', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify({ id: 'user-1' }), { status: 200 }),
    );

    await getCurrentUser('session=abc');

    expect(fetch).toHaveBeenCalledWith(`${gatewayUrl}/api/auth/me`, {
      headers: {
        Cookie: 'session=abc',
      },
      cache: 'no-store',
    });
  });

  it('returns null when the auth endpoint responds unsuccessfully', async () => {
    vi.mocked(fetch).mockResolvedValue(new Response(null, { status: 401 }));

    await expect(getCurrentUser('session=expired')).resolves.toBeNull();
  });
});
