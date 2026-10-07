import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { NextRequest } from 'next/server';

import { GET as getUsers } from '@/app/api/users/route';
import { getAllUsers } from '@/lib/api/users';

describe('Users API', () => {
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

  it('forwards the authenticated users request and cookie to the gateway', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify([{ id: 'user-1', username: 'alice' }]), {
        status: 200,
      }),
    );

    const request = new NextRequest('http://localhost/api/users', {
      headers: { cookie: 'auth=session-cookie' },
    });

    const response = await getUsers(request);

    expect(response.status).toBe(200);
    expect(fetch).toHaveBeenCalledWith(`${gatewayUrl}/api/users`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        cookie: 'auth=session-cookie',
      },
    });
  });

  it('returns a server error when the gateway request fails', async () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined);
    vi.mocked(fetch).mockRejectedValue(new Error('Gateway unavailable'));

    const response = await getUsers(
      new NextRequest('http://localhost/api/users'),
    );

    expect(response.status).toBe(500);
    await expect(response.json()).resolves.toEqual({
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while fetching users.',
    });
  });

  it('normalizes gateway user IDs for the frontend user shape', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(
        JSON.stringify([{ id: 'identity-user-1', username: 'alice' }]),
        {
          status: 200,
        },
      ),
    );

    await expect(getAllUsers()).resolves.toEqual([
      { user_id: 'identity-user-1', username: 'alice' },
    ]);
  });
});
