import { NextRequest } from 'next/server';

type RouteContext = {
  params: Promise<{ groupchat_id: string; message_id: string }>;
};

export async function PUT(request: NextRequest, { params }: RouteContext) {
  try {
    const { groupchat_id, message_id } = await params;
    const requestBody = await request.json();
    const response = await fetch(
      `${process.env.GATEWAY_URL}/api/groupchats/${groupchat_id}/messages/${message_id}`,
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
    console.error('Error updating message:', error);
    return new Response(
      JSON.stringify({
        title: 'Server Error',
        status: 500,
        detail: 'An error occurred while updating the message.',
      }),
      {
        status: 500,
        headers: {
          'Content-Type': 'application/json',
        },
      },
    );
  }
}
