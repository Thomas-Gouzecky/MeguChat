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

export async function createNewMessage(
  groupchat_id: string,
  content: string,
): Promise<MessageType | ErrorResponse> {
  try {
    const response = await fetch(`/api/groupchats/${groupchat_id}/messages`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify({ content }),
    });

    const payload = await response.json();

    if (!response.ok) {
      return payload as ErrorResponse;
    }

    return payload as MessageType;
  } catch (error) {
    console.error('Error creating new message:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while creating a new message.',
    } as ErrorResponse;
  }
}

export async function updateMessage(
  groupchat_id: string,
  message_id: string,
  content: string,
): Promise<MessageType | ErrorResponse> {
  try {
    const response = await fetch(
      `/api/groupchats/${groupchat_id}/messages/${message_id}`,
      {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({ content }),
      },
    );

    const payload = await response.json();

    if (!response.ok) {
      return payload as ErrorResponse;
    }

    return payload as MessageType;
  } catch (error) {
    console.error('Error updating message:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while updating the message.',
    } as ErrorResponse;
  }
}
