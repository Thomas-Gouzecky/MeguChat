import { Register } from '@/lib/auth/register';

export async function POST(request: Request): Promise<Response> {
  try {
    const body: AuthRequest = await request.json();
    return await Register(body);
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
