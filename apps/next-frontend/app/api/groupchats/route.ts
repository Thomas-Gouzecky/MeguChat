import type { NextRequest } from 'next/server';

export async function GET(request: NextRequest): Promise<Response> {
  try {
    const response = await fetch(`${process.env.GATEWAY_URL}/api/groupchats`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        cookie: request.headers.get('cookie') ?? '',
      },
      cache: 'no-store',
    });

    return response;
  } catch (error) {
    console.error('Error fetching group chats:', error);
    return new Response(
      JSON.stringify({
        title: 'Server Error',
        status: 500,
        detail: 'An error occurred while fetching group chats.',
      }),
      {
        status: 500,
        headers: { 'Content-Type': 'application/json' },
      },
    );
  }
}
