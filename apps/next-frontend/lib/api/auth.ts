export async function Login(request: AuthRequest): Promise<AuthResponse> {
  try {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
      credentials: 'include',
    });

    return (await response.json()) as AuthResponse;
  } catch (error) {
    console.error('Error logging in:', error);
    return {
      isSuccess: false,
      errorMessage: 'An error occurred while logging in.',
    };
  }
}

export async function Register(request: AuthRequest): Promise<AuthResponse> {
  try {
    const response = await fetch('/api/auth/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });

    return (await response.json()) as AuthResponse;
  } catch (error) {
    console.error('Error registering:', error);
    return {
      isSuccess: false,
      errorMessage: 'An error occurred while registering.',
    };
  }
}