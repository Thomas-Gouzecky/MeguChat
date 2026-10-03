'use server';

export async function Login(request: AuthRequest): Promise<Response> {
  const response = await fetch(`${process.env.GATEWAY_URL}/api/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
    credentials: 'include',
  });

  return new Response(await response.text(), {
    status: response.status,
    headers: response.headers,
  });
}
