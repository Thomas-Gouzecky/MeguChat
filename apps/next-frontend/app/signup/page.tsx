import { SignupForm } from '@meguchat/ui/components/signup-form';

export default function SignupPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background">
      <SignupForm className="mx-auto w-full max-w-sm" />
    </div>
  );
}
