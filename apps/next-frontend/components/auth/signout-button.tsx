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
    <div className={cn(className)}>
      <Button
        type="button"
        size="icon"
        variant="ghost"
        onClick={handleSignOut}
        className="transition-none active:translate-y-0!"
        {...props}
      >
        <LogOut className="h-4 w-4" />
      </Button>
    </div>
  );
}
