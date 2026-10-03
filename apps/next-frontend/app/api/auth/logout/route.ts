import { Logout } from '@/lib/auth/logout';

export async function POST(request: Request): Promise<Response> {
  try {
    return await Logout(request.headers.get('cookie') ?? '');
  } catch (error) {
    console.error('Error logging out:', error);
    return Response.json(
      {
        isSuccess: false,
        errorMessage: 'An error occurred while logging out.',
      },
      { status: 500 },
    );
  }
}
