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

export async function POST(request: NextRequest): Promise<Response> {
  try {
    const requestBody = await request.json();
    const response = await fetch(`${process.env.GATEWAY_URL}/api/groupchats`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        cookie: request.headers.get('cookie') ?? '',
      },
      body: JSON.stringify(requestBody),
    });

    return response;
  } catch (error) {
    console.error('Error creating group chat:', error);
    return new Response(
      JSON.stringify({
        title: 'Server Error',
        status: 500,
        detail: 'An error occurred while creating group chat.',
      }),
      {
        status: 500,
        headers: { 'Content-Type': 'application/json' },
      },
    );
  }
}
