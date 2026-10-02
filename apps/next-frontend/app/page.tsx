import { LoginForm } from '@meguchat/ui/components/login-form';
import { SignupForm } from '@meguchat/ui/components/signup-form';

export default function Home() {
  return (
    <>
      <LoginForm className="mx-auto w-full max-w-sm" />
      <SignupForm className="mx-auto w-full max-w-sm" />
    </>
  );
}
