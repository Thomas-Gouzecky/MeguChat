type AuthValidationError = {
  type: 'backend-unavailable' | 'username' | 'password' | 'confirm-password';
  message: string;
};
