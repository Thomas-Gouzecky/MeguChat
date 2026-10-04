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
import { Register } from '../lib/api/auth';
import { redirect } from 'next/navigation';
import validateForm from '../lib/validateForm';

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

    redirect('/login');
  }

  const getError = (inputField: AuthError['inputField']) =>
    errors.find((currentError) => currentError.inputField === inputField);

  const usernameError = getError('username');
  const passwordError = getError('password');
  const generalError = getError('general');

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
                <FieldDescription
                  id="username-error"
                  role="alert"
                  className="text-destructive"
                >
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
                <FieldDescription
                  id="password-error"
                  role="alert"
                  className="text-destructive"
                >
                  {passwordError.description}
                </FieldDescription>
              ) : (
                <FieldDescription>
                  Use 8+ characters with uppercase, lowercase, a number, and a
                  symbol.
                </FieldDescription>
              )}
            </Field>
            <Field
              data-invalid={errors.some(
                (currentError) =>
                  currentError.inputField === 'confirm-password',
              )}
            >
              <FieldLabel htmlFor="confirm-password">
                Confirm Password
              </FieldLabel>
              <PasswordInput
                id="confirm-password"
                name="confirm-password"
                aria-invalid={errors.some(
                  (currentError) =>
                    currentError.inputField === 'confirm-password',
                )}
              />
              {errors.some(
                (currentError) =>
                  currentError.inputField === 'confirm-password',
              ) ? (
                <FieldDescription
                  id="confirm-password-error"
                  role="alert"
                  className="text-destructive"
                >
                  {
                    errors.find(
                      (currentError) =>
                        currentError.inputField === 'confirm-password',
                    )?.description
                  }
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
