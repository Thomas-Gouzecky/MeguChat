'use client';

export async function getUsersGroupchats(): Promise<
  Groupchat[] | ErrorResponse
> {
  try {
    const response = await fetch('/api/groupchats', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
    });

    if (response.ok) {
      const groupchats = (await response.json()) as Array<{
        groupchat_id: number;
        name: string;
        created_at?: string;
      }>;

      return groupchats.map((groupchat) => ({
        id: groupchat.groupchat_id,
        name: groupchat.name,
        created_at: groupchat.created_at,
      }));
    }

    return (await response.json()) as ErrorResponse;
  } catch (error) {
    console.error('Error fetching group chats:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while fetching group chats.',
    } as ErrorResponse;
  }
}
