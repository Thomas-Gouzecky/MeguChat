export async function Login(request: AuthRequest): Promise<LoginResponse> {
  try {
    const response = await fetch(
      `${process.env.NEXT_PUBLIC_GATEWAY_URL}/api/auth/login`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      },
    );

    const data: LoginResponse = await response.json();

    return data;
  } catch (error) {
    console.error('Error logging in:', error);
    return {
      isSuccess: false,
      errorMessage: 'An error occurred while logging in.',
    };
  }
}
