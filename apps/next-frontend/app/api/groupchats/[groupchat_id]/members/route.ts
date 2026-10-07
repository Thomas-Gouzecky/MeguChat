import { NextRequest } from 'next/server';

type RouteContext = {
  params: Promise<{ groupchat_id: string }>;
};

export async function GET(request: NextRequest, { params }: RouteContext) {
  try {
    const { groupchat_id } = await params;
    const response = await fetch(
      `${process.env.GATEWAY_URL}/api/groupchats/${groupchat_id}/members`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          cookie: request.headers.get('cookie') ?? '',
        },
      },
    );

    return response;
  } catch (error) {
    console.error('Error fetching group chat members:', error);
    return new Response(
      JSON.stringify({
        title: 'Server Error',
        status: 500,
        detail: 'An error occurred while fetching the group chat members.',
      }),
      {
        status: 500,
        headers: { 'Content-Type': 'application/json' },
      },
    );
  }
}
