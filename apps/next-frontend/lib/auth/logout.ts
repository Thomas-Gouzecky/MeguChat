'use server';

export async function Logout(cookie: string): Promise<Response> {
  const response = await fetch(`${process.env.GATEWAY_URL}/api/auth/logout`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Cookie: cookie,
    },
  });
  return response;
}
