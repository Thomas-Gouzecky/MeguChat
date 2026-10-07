import { NextRequest } from 'next/server';

type RouteContext = {
  params: Promise<{ groupchat_id: string }>;
};

export async function DELETE(
  request: NextRequest,
  { params }: RouteContext,
): Promise<Response> {
  try {
    const { groupchat_id } = await params;
    const response = await fetch(
      `${process.env.GATEWAY_URL}/api/groupchats/${groupchat_id}`,
      {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
          cookie: request.headers.get('cookie') ?? '',
        },
      },
    );

    return response;
  } catch (error) {
    console.error('Error deleting group chat:', error);
    return new Response(
      JSON.stringify({
        title: 'Server Error',
        status: 500,
        detail: 'An error occurred while deleting group chat.',
      }),
      {
        status: 500,
        headers: { 'Content-Type': 'application/json' },
      },
    );
  }
}

export async function PUT(
  request: NextRequest,
  { params }: RouteContext,
): Promise<Response> {
  try {
    const { groupchat_id } = await params;

    const requestBody = await request.json();
    const response = await fetch(
      `${process.env.GATEWAY_URL}/api/groupchats/${groupchat_id}`,
      {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          cookie: request.headers.get('cookie') ?? '',
        },
        body: JSON.stringify(requestBody),
      },
    );
    return response;
  } catch (error) {
    console.error('Error editing group chat:', error);
    return new Response(
      JSON.stringify({
        title: 'Server Error',
        status: 500,
        detail: 'An error occurred while editing group chat.',
      }),
      {
        status: 500,
        headers: { 'Content-Type': 'application/json' },
      },
    );
  }
}
