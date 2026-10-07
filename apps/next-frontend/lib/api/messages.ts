'use client';

export async function getMessagesByGroupchatId(
  groupchatId: number,
): Promise<MessageType[] | ErrorResponse> {
  try {
    const response = await fetch(`/api/groupchats/${groupchatId}/messages`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
    });

    const payload = await response.json();

    if (!response.ok) {
      return payload as ErrorResponse;
    }

    return payload as MessageType[];
  } catch (error) {
    console.error('Error fetching messages:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while fetching messages.',
    } as ErrorResponse;
  }
}
