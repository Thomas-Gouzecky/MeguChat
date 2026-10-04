// AuthResponse is a type that represents the response from authentication-related API calls. It can be defined as follows:
type AuthResponse = {
  isSuccess: boolean;
  errorMessage?: string;
  errors?: AuthError[];
};

type AuthError = {
  code: string;
  description: string;
  inputField: 'username' | 'password' | 'confirm-password' | 'general';
};

// AuthRequest is a type that represents the request body for authentication-related API calls. It can be defined as follows:
type AuthRequest = {
  username: string;
  password: string;
};
