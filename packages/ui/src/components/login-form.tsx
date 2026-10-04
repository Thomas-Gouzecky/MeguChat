'use client';

import { cn } from 'cn';

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

import { redirect } from 'next/navigation';
import { useState } from 'react';

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<'div'>) {
  const [error, setError] = useState<AuthValidationError | null>(null);

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>,
  ): Promise<void> {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const username = formData.get('username') as string;
    const password = formData.get('password') as string;

    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username, password }),
      credentials: 'include',
    });

    if (!response.ok) {
      try {
        const errorData = await response.json();
        const errorMessage = {
          errorMessage:
            errorData.errorMessage ?? errorData.ErrorMessage ?? 'Login failed.',
        };

        const parsedError = {
          type: 'username',
          message: errorMessage.errorMessage,
        } as AuthValidationError;

        setError(parsedError);
        return;
      } catch {
        // Keep the fallback when the API response has no JSON body.
      }
    }

    setError(null);

    redirect('/');
  }

  return (
    <div className={cn('flex flex-col gap-6', className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle>Login to your account</CardTitle>
          <CardDescription>
            Enter your username below to login to your account
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit}>
            <FieldGroup>
              <Field
                data-invalid={error?.type === 'username' ? 'true' : 'false'}
              >
                <FieldLabel htmlFor="username">Username</FieldLabel>
                <Input
                  aria-invalid={error?.type === 'username' ? 'true' : 'false'}
                  id="username"
                  name="username"
                  type="text"
                  placeholder="Your username"
                  required
                />
                {error?.type === 'username' && (
                  <FieldDescription
                    id="username-error"
                    className="text-destructive"
                  >
                    {error?.message}
                  </FieldDescription>
                )}
              </Field>
              <Field
                data-invalid={error?.type === 'password' ? 'true' : 'false'}
              >
                <div className="flex items-center">
                  <FieldLabel htmlFor="password">Password</FieldLabel>
                  <Link
                    href="/LOL"
                    className="ml-auto inline-block text-sm underline-offset-4 hover:underline"
                  >
                    Forgot your password?
                  </Link>
                </div>
                <PasswordInput
                  aria-invalid={error?.type === 'password' ? 'true' : 'false'}
                  id="password"
                  name="password"
                  required
                />
                {error?.type === 'password' && (
                  <FieldDescription
                    id="password-error"
                    className="text-destructive"
                  >
                    {error?.message}
                  </FieldDescription>
                )}
              </Field>
              <Field>
                <Button type="submit">Login</Button>
                <Button variant="outline" type="button">
                  Login with Google
                </Button>
                <FieldDescription className="text-center">
                  Don&apos;t have an account?{' '}
                  <Link href="/signup">Sign up</Link>
                </FieldDescription>
              </Field>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
