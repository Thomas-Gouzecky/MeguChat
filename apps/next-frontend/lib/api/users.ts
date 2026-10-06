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

    if (!response.ok) {
      return (await response.json()) as ErrorResponse;
    }

    const users = (await response.json()) as User[];

    return users.map((user) => ({
      user_id: user.user_id,
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
