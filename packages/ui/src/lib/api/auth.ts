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

    const responseData: AuthResponse = await response.json();

    return responseData;
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

    const responseData: AuthResponse = await response.json();

    return responseData;
  } catch (error) {
    console.error('Error registering:', error);
    return {
      isSuccess: false,
      errorMessage: 'An error occurred while registering.',
    };
  }
}
