import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { getCurrentUser } from '@/lib/auth/auth';

import { cookies } from 'next/headers';

vi.mock('next/headers', () => ({
  cookies: vi.fn(),
}));
describe('getCurrentUser', () => {
  const gatewayUrl = 'http://localhost:8080';

  beforeEach(() => {
    process.env.GATEWAY_URL = gatewayUrl;
    vi.stubGlobal('fetch', vi.fn());
    vi.mocked(cookies).mockResolvedValue({
      toString: () => 'session=abc',
    } as Awaited<ReturnType<typeof cookies>>);
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

    const currentUser = await getCurrentUser();

    expect(currentUser).toEqual(user);
  });

  it('calls the auth endpoint with the cookie and no-store cache policy', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify({ id: 'user-1' }), { status: 200 }),
    );

    const user = await getCurrentUser();

    expect(fetch).toHaveBeenCalledWith(`${gatewayUrl}/api/auth/me`, {
      headers: {
        Cookie: 'session=abc',
      },
      cache: 'no-store',
    });

    expect(user).toEqual({ id: 'user-1' });
  });

  it('uses the request cookie when calling the auth endpoint', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify({ id: 'user-1' }), {
        status: 200,
      }),
    );

    await getCurrentUser();

    expect(fetch).toHaveBeenCalledWith(
      `${gatewayUrl}/api/auth/me`,
      expect.objectContaining({
        headers: {
          Cookie: 'session=abc',
        },
      }),
    );
  });

  it('returns null when the auth endpoint responds unsuccessfully', async () => {
    vi.mocked(fetch).mockResolvedValue(new Response(null, { status: 401 }));

    const user = await getCurrentUser();

    expect(user).toBeNull();
  });
});
