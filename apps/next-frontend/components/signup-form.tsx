'use client';

import Link from 'next/link';
import { redirect } from 'next/navigation';
import { useState } from 'react';

import { Button } from '@meguchat/ui/components/ui/button';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@meguchat/ui/components/ui/card';
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
} from '@meguchat/ui/components/ui/field';
import { Input } from '@meguchat/ui/components/ui/input';
import { PasswordInput } from '@meguchat/ui/components/ui/password-input';
import { Register } from '@/lib/api/auth';
import validateForm from '@/lib/validateForm';

export function SignupForm({ ...props }: React.ComponentProps<typeof Card>) {
  const [errors, setErrors] = useState<AuthError[]>([]);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setErrors([]);
    const formData = new FormData(event.currentTarget);
    const username = formData.get('username') as string;
    const password = formData.get('password') as string;
    const confirmPassword = formData.get('confirm-password') as string;

    const formErrors = validateForm(username, password, confirmPassword);
    if (formErrors.length > 0) {
      setErrors(formErrors);
      return;
    }

    const response = await Register({ username, password });
    if (response.errors && response.errors.length > 0) {
      setErrors(response.errors);
      return;
    }

    if (!response.isSuccess) {
      setErrors([
        {
          code: 'signup-failed',
          description: response.errorMessage ?? 'Unable to create account.',
          inputField: 'general',
        },
      ]);
      return;
    }

    redirect('/login');
  }

  const getError = (inputField: AuthError['inputField']) =>
    errors.find((error) => error.inputField === inputField);
  const usernameError = getError('username');
  const passwordError = getError('password');
  const generalError = getError('general');
  const confirmPasswordError = errors.find(
    (error) => error.inputField === 'confirm-password',
  );

  return (
    <Card {...props}>
      <CardHeader>
        <CardTitle>Create an account</CardTitle>
        <CardDescription>
          Enter your information below to create your account
        </CardDescription>
      </CardHeader>
      <CardContent>
        {generalError && (
          <FieldDescription
            role="alert"
            className="text-destructive bg-accent border p-2 rounded-md mb-4"
          >
            {generalError.description}
          </FieldDescription>
        )}
        <form onSubmit={handleSubmit}>
          <FieldGroup>
            <Field data-invalid={usernameError ? 'true' : 'false'}>
              <FieldLabel htmlFor="username">Username</FieldLabel>
              <Input
                id="username"
                name="username"
                type="text"
                placeholder="Your username"
                aria-invalid={usernameError ? 'true' : 'false'}
              />
              {usernameError ? (
                <FieldDescription id="username-error" role="alert">
                  {usernameError.description}
                </FieldDescription>
              ) : (
                <FieldDescription>
                  This is your public display name.
                </FieldDescription>
              )}
            </Field>
            <Field data-invalid={passwordError ? 'true' : 'false'}>
              <FieldLabel htmlFor="password">Password</FieldLabel>
              <PasswordInput
                id="password"
                name="password"
                aria-invalid={passwordError ? 'true' : 'false'}
              />
              {passwordError ? (
                <FieldDescription id="password-error" role="alert">
                  {passwordError.description}
                </FieldDescription>
              ) : (
                <FieldDescription>
                  Use 8+ characters with uppercase, lowercase, a number, and a
                  symbol.
                </FieldDescription>
              )}
            </Field>
            <Field data-invalid={confirmPasswordError ? 'true' : 'false'}>
              <FieldLabel htmlFor="confirm-password">
                Confirm Password
              </FieldLabel>
              <PasswordInput
                id="confirm-password"
                name="confirm-password"
                aria-invalid={confirmPasswordError ? 'true' : 'false'}
              />
              {confirmPasswordError ? (
                <FieldDescription id="confirm-password-error" role="alert">
                  {confirmPasswordError.description}
                </FieldDescription>
              ) : (
                <FieldDescription>
                  Please confirm your password.
                </FieldDescription>
              )}
            </Field>
            <Field>
              <Button type="submit">Create Account</Button>
              <Button variant="outline" type="button">
                Sign up with Google
              </Button>
              <FieldDescription className="px-6 text-center">
                Already have an account? <Link href="/login">Sign in</Link>
              </FieldDescription>
            </Field>
          </FieldGroup>
        </form>
      </CardContent>
    </Card>
  );
}
