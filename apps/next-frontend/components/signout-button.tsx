'use client';

import { LogOut } from 'lucide-react';
import { useRouter } from 'next/navigation';

import { Button } from '@meguchat/ui/components/ui/button';
import { cn } from 'cn';

export function SignOutButton({
  className,
  ...props
}: React.ComponentProps<'button'>) {
  const router = useRouter();

  async function handleSignOut(): Promise<void> {
    const response = await fetch('/api/auth/logout', {
      method: 'POST',
      credentials: 'include',
    });

    if (!response.ok) {
      alert('Sign out failed');
      return;
    }

    router.push('/login');
    router.refresh();
  }

  return (
    <div className={cn('flex flex-col gap-6', className)}>
      <Button
        type="button"
        variant="ghost"
        onClick={handleSignOut}
        className={cn('w-full justify-start', className)}
        {...props}
      >
        <LogOut className="h-4 w-4" />
        Sign Out
      </Button>
    </div>
  );
}