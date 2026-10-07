import { NextRequest } from 'next/server';

export async function GET(request: NextRequest) {
  try {
    const response = await fetch(`${process.env.GATEWAY_URL}/api/users`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        cookie: request.headers.get('cookie') ?? '',
      },
    });

    return response;
  } catch (error) {
    console.error('Error fetching users:', error);
    return new Response(
      JSON.stringify({
        title: 'Server Error',
        status: 500,
        detail: 'An error occurred while fetching users.',
      }),
      {
        status: 500,
        headers: { 'Content-Type': 'application/json' },
      },
    );
  }
}
