'use client';

import Link from 'next/link';
import { useRouter } from 'next/navigation';
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
import { cn } from '@meguchat/ui/lib/utils';

import { Login } from '@/lib/api/auth';
import validateForm from '@/lib/validateForm';
import { setUser } from '@/store/slices/authSlice';
import { AppDispatch } from '@/store/store';
import { useDispatch } from 'react-redux';

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<'div'>) {
  const [errors, setErrors] = useState<AuthError[]>([]);
  const router = useRouter();
  const dispatch = useDispatch<AppDispatch>();

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>,
  ): Promise<void> {
    event.preventDefault();
    setErrors([]);
    const formData = new FormData(event.currentTarget);
    const username = formData.get('username') as string;
    const password = formData.get('password') as string;

    const formErrors = validateForm(username, password);
    if (formErrors.length > 0) {
      setErrors(formErrors);
      return;
    }

    const response = await Login({ username, password });
    if (response.errors && response.errors.length > 0) {
      setErrors(response.errors);
      return;
    }

    if (!response.isSuccess) {
      setErrors([
        {
          code: 'login-failed',
          description: response.errorMessage ?? 'Unable to log in.',
          inputField: 'general',
        },
      ]);
      return;
    }

    if (response.userId && response.username) {
      dispatch(
        setUser({
          user_id: response.userId,
          username: response.username,
        }),
      );
    }

    router.push('/');
    router.refresh();
  }

  const getError = (inputField: AuthError['inputField']) =>
    errors.find((error) => error.inputField === inputField);
  const usernameError = getError('username');
  const passwordError = getError('password');
  const generalError = getError('general');

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
            {generalError && (
              <FieldDescription
                role="alert"
                className="text-destructive bg-accent border p-2 rounded-md mb-4"
              >
                {generalError.description}
              </FieldDescription>
            )}
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
                {usernameError && (
                  <FieldDescription id="username-error" role="alert">
                    {usernameError.description}
                  </FieldDescription>
                )}
              </Field>
              <Field data-invalid={passwordError ? 'true' : 'false'}>
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
                  aria-invalid={passwordError ? 'true' : 'false'}
                  id="password"
                  name="password"
                  required
                />
                {passwordError && (
                  <FieldDescription id="password-error" role="alert">
                    {passwordError.description}
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
