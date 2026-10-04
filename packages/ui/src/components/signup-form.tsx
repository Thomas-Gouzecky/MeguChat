'use client';

import { useState } from 'react';
import { Button } from './ui/button';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from './ui/card';
import { Field, FieldDescription, FieldGroup, FieldLabel } from './ui/field';
import { Input } from './ui/input';
import { PasswordInput } from './ui/password-input';

import Link from 'next/link';

export function SignupForm({ ...props }: React.ComponentProps<typeof Card>) {
  const [error, setError] = useState<AuthValidationError | null>(null);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const username = formData.get('username') as string;
    const password = formData.get('password') as string;
    const confirmPassword = formData.get('confirm-password') as string;

    if (password !== confirmPassword) {
      setError({
        type: 'confirm-password',
        message: 'Passwords do not match.',
      });
      return;
    }

    const response = await fetch('/api/auth/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username: username, password: password }),
      credentials: 'include',
    });

    if (!response.ok) {
      try {
        const errorData = await response.json();
        const errorMessage =
          errorData.errorMessage ??
          errorData.ErrorMessage ??
          'Registration failed.';

        const parsedError = {
          type: errorData.type ?? 'backend-unavailable',
          message: errorMessage,
        } as AuthValidationError;

        setError(parsedError);
        return;
      } catch {
        // Keep the fallback when the API response has no JSON body.
      }
    }

    setError(null);
  }

  return (
    <Card {...props}>
      <CardHeader>
        <CardTitle>Create an account</CardTitle>
        <CardDescription>
          Enter your information below to create your account
        </CardDescription>
      </CardHeader>
      <CardContent>
        {error?.type === 'backend-unavailable' && (
          <FieldDescription
            role="alert"
            className="text-destructive bg-accent border p-2 rounded-md mb-4"
          >
            The backend is currently unavailable. Please try again later.
          </FieldDescription>
        )}
        <form onSubmit={handleSubmit}>
          <FieldGroup>
            <Field data-invalid={error?.type === 'username' ? 'true' : 'false'}>
              <FieldLabel htmlFor="username">Username</FieldLabel>
              <Input
                id="username"
                name="username"
                type="text"
                placeholder="Your username"
              />
              {error?.type === 'username' ? (
                <FieldDescription
                  id="username-error"
                  role="alert"
                  className="text-destructive"
                >
                  {error.message}
                </FieldDescription>
              ) : (
                <FieldDescription>
                  This is your public display name.
                </FieldDescription>
              )}
            </Field>
            <Field data-invalid={error?.type === 'password' ? 'true' : 'false'}>
              <FieldLabel htmlFor="password">Password</FieldLabel>
              <PasswordInput
                id="password"
                name="password"
                aria-invalid={error?.type === 'password' ? 'true' : 'false'}
              />
              {error?.type === 'password' ? (
                <FieldDescription
                  id="password-error"
                  role="alert"
                  className="text-destructive"
                >
                  {error.message}
                </FieldDescription>
              ) : (
                <FieldDescription>
                  Use 8+ characters with uppercase, lowercase, a number, and a
                  symbol.
                </FieldDescription>
              )}
            </Field>
            <Field
              data-invalid={
                error?.type === 'confirm-password' ? 'true' : 'false'
              }
            >
              <FieldLabel htmlFor="confirm-password">
                Confirm Password
              </FieldLabel>
              <PasswordInput
                id="confirm-password"
                name="confirm-password"
                aria-invalid={
                  error?.type === 'confirm-password' ? 'true' : 'false'
                }
              />
              {error?.type === 'confirm-password' ? (
                <FieldDescription
                  id="confirm-password-error"
                  role="alert"
                  className="text-destructive"
                >
                  {error.message}
                </FieldDescription>
              ) : (
                <FieldDescription>
                  Please confirm your password.
                </FieldDescription>
              )}
            </Field>
            <FieldGroup>
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
          </FieldGroup>
        </form>
      </CardContent>
    </Card>
  );
}
