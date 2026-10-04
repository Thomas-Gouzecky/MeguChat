import { LoginForm } from '@/components/login-form';

export default function LoginPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background">
      <LoginForm className="mx-auto w-full max-w-sm" />
    </div>
  );
}
