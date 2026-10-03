'use server';

export async function Register(request: AuthRequest): Promise<Response> {
  const response = await fetch(`${process.env.GATEWAY_URL}/api/auth/register`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  });

  return new Response(await response.text(), {
    status: response.status,
    headers: response.headers,
  });
}
