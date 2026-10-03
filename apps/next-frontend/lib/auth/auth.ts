'use server';

export async function getCurrentUser(cookie: string) {
  const response = await fetch(`${process.env.GATEWAY_URL}/api/auth/me`, {
    headers: {
      Cookie: cookie,
    },
    cache: 'no-store',
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}
