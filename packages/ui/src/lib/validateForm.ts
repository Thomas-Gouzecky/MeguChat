export default function validateForm(
  username: string,
  password: string,
  confirmPassword?: string,
): AuthError[] {
  const errors: AuthError[] = [];

  if (!username) {
    errors.push({
      code: 'username-required',
      description: 'Username is required.',
      inputField: 'username',
    });
  }

  if (!password) {
    errors.push({
      code: 'password-required',
      description: 'Password is required.',
      inputField: 'password',
    });
  }

  if (confirmPassword && password !== confirmPassword) {
    errors.push({
      code: 'passwords-do-not-match',
      description: 'Passwords do not match.',
      inputField: 'confirm-password',
    });
  }

  return errors;
}
