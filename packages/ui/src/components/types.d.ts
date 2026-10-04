type AuthValidationError = {
  type: 'username' | 'password';
  message: string;
};
