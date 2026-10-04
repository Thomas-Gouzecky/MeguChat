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

  const responseData: AuthResponse = await response.json();

  return new Response(JSON.stringify(responseData), {
    status: response.status,
    headers: response.headers,
  });
}
