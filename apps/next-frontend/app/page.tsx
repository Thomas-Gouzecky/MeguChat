import { LoginForm } from '@meguchat/ui/components/login-form';
import { SignupForm } from '@meguchat/ui/components/signup-form';
import { ModeToggle } from '@meguchat/ui/components/mode-toggle';

export default function Home() {
  return (
    <>
      <ModeToggle />
      <LoginForm className="mx-auto w-full max-w-sm" />
      <SignupForm className="mx-auto w-full max-w-sm" />
    </>
  );
}
