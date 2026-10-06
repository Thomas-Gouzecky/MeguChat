'use client';

export async function getAllUsers(): Promise<User[] | ErrorResponse> {
  try {
    const response = await fetch('/api/users', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
    });

    const body = await response.text();

    if (!body.trim()) {
      if (!response.ok) {
        return {
          title: 'Unable to fetch users',
          status: response.status,
          detail: 'The users endpoint returned an empty response.',
        };
      }

      return [];
    }

    const payload: unknown = JSON.parse(body);

    if (!response.ok) {
      return payload as ErrorResponse;
    }

    const users = payload as Array<{
      id: string;
      username: string;
    }>;

    return users.map((user) => ({
      user_id: user.id,
      username: user.username,
    }));
  } catch (error) {
    console.error('Error fetching users:', error);
    return {
      title: 'Server Error',
      status: 500,
      detail: 'An error occurred while fetching users.',
    } as ErrorResponse;
  }
}
