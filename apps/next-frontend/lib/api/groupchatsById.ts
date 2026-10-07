'use client';

export async function getGroupchatById(
  groupchatId: string,
): Promise<Groupchat | ErrorResponse> {
  try {
    const response = await fetch(`/api/groupchats/${groupchatId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
    });
    if (!response.ok) {
      const errorPayload = await response.json();
      return errorPayload as ErrorResponse;
    }
    const groupchat = await response.json();
    return groupchat as Groupchat;
  } catch (error) {
    console.error('Error fetching groupchat by ID:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while fetching group chat by ID.',
    } as ErrorResponse;
  }
}
