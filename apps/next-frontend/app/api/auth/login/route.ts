import { Login } from '@/lib/auth/login';

export async function POST(request: Request): Promise<Response> {
  try {
    const body: AuthRequest = await request.json();
    return await Login(body);
  } catch (error) {
    console.error('Error logging in:', error);
    return Response.json(
      {
        isSuccess: false,
        errorMessage: 'An error occurred while logging in.',
      },
      { status: 500 },
    );
  }
}
