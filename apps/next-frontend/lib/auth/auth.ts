'use server';

import { cookies } from 'next/headers';

export async function getCurrentUser() {
  try {
    const cookieStore = await cookies();

    const response = await fetch(`${process.env.GATEWAY_URL}/api/auth/me`, {
      headers: {
        Cookie: cookieStore.toString(),
      },
      cache: 'no-store',
    });

    if (!response.ok) {
      return null;
    }

    return response.json();
  } catch (error) {
    console.error('Error fetching current user:', error);
    return null;
  }
}
