// AuthResponse is a type that represents the response from authentication-related API calls. It can be defined as follows:
type AuthResponse = {
  isSuccess: boolean;
  errorMessage?: string;
  errors?: AuthError[];
};

type AuthError = {
  code: string;
  description: string;
  inputField: 'username' | 'password' | 'general';
};

// AuthRequest is a type that represents the request body for authentication-related API calls. It can be defined as follows:
type AuthRequest = {
  username: string;
  password: string;
};

// LoginResponse is a type that represents the response from the login API. It can be defined as follows:
type LoginResponse = AuthResponse;

// RegisterResponse is a type that represents the response from the register API. It can be defined as follows:
type RegisterResponse = AuthResponse;
