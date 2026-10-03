export async function Register(request: AuthRequest) {
  try {
    const response = await fetch(
      `${process.env.NEXT_PUBLIC_GATEWAY_URL}/api/auth/register`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      },
    );

    const data: RegisterResponse = await response.json();

    return data;
  } catch (error) {
    console.error('Error registering:', error);
    return {
      isSuccess: false,
      errorMessage: 'An error occurred while registering.',
    };
  }
}
