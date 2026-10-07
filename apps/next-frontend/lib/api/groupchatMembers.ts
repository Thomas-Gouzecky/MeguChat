'use client';

export async function getGroupchatMembers(
  groupchatId: number,
): Promise<GroupchatMember[] | ErrorResponse> {
  try {
    const response = await fetch(`/api/groupchats/${groupchatId}/members`, {
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

    const groupchatMembers = payload as GroupchatMember[];

    return groupchatMembers;
  } catch (error) {
    console.error('Error fetching group chat members:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while fetching the group chat members.',
    } as ErrorResponse;
  }
}
