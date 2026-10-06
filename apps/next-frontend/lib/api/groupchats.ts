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
      return (await response.json()) as ErrorResponse;
    }

    const groupchats = (await response.json()) as Groupchat[];

    return groupchats.map((groupchat) => ({
      groupchat_id: groupchat.groupchat_id,
      name: groupchat.name,
      created_at: groupchat.created_at,
    }));
  } catch (error) {
    console.error('Error fetching group chats:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while fetching group chats.',
    } as ErrorResponse;
  }
}

export async function createGroupchat(
  CreateGroupchatRequest: CreateGroupchatRequest,
): Promise<Groupchat | ErrorResponse> {
  try {
    const response = await fetch('/api/groupchats', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify({
        name: CreateGroupchatRequest.name,
        users: CreateGroupchatRequest.users,
      }),
    });

    if (!response.ok) {
      return (await response.json()) as ErrorResponse;
    }

    const groupchat = (await response.json()) as Groupchat;

    return {
      groupchat_id: groupchat.groupchat_id,
      name: groupchat.name,
      created_at: groupchat.created_at,
    };
  } catch (error) {
    console.error('Error creating group chat:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while creating the group chat.',
    } as ErrorResponse;
  }
}

export async function deleteGroupchat(
  groupchatId: number,
): Promise<Groupchat | ErrorResponse> {
  try {
    const response = await fetch(`/api/groupchats/${groupchatId}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
    });

    if (!response.ok) {
      return (await response.json()) as ErrorResponse;
    }

    const groupchat = (await response.json()) as Groupchat;

    return {
      groupchat_id: groupchat.groupchat_id,
      name: groupchat.name,
      created_at: groupchat.created_at,
    };
  } catch (error) {
    console.error('Error deleting group chat:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while deleting the group chat.',
    } as ErrorResponse;
  }
}

export async function editGroupchat(
  groupchatId: number,
  updatedGroupchat: { name?: string; users?: string[] },
): Promise<Groupchat | ErrorResponse> {
  try {
    const response = await fetch(`/api/groupchats/${groupchatId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(updatedGroupchat),
    });

    if (!response.ok) {
      return (await response.json()) as ErrorResponse;
    }

    const groupchat = (await response.json()) as Groupchat;

    return {
      groupchat_id: groupchat.groupchat_id,
      name: groupchat.name,
      created_at: groupchat.created_at,
    };
  } catch (error) {
    console.error('Error editing group chat:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while editing the group chat.',
    } as ErrorResponse;
  }
}
