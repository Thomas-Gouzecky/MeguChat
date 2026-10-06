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

    if (!response.ok) {
      return (await response.json()) as ErrorResponse;
    }

    const groupchatMembers = (await response.json()) as GroupchatMember[];

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
