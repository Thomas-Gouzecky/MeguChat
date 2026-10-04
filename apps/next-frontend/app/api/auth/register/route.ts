import { Register } from '@/lib/auth/register';

export async function POST(request: Request): Promise<Response> {
  try {
    const requestBody = await request.json();

    const authRequest: AuthRequest = {
      username: requestBody.username,
      password: requestBody.password,
    };

    const response = await Register(authRequest);

    return response;
  } catch (error) {
    console.error('Error registering:', error);
    return Response.json(
      {
        isSuccess: false,
        errorMessage: 'An error occurred while registering.',
      },
      { status: 500 },
    );
  }
}
