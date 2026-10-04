import { Login } from '@/lib/auth/login';

export async function POST(request: Request): Promise<Response> {
  try {
    const requestBody = await request.json();

    const authRequest: AuthRequest = {
      username: requestBody.username,
      password: requestBody.password,
    };

    const response = await Login(authRequest);

    return response;
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
