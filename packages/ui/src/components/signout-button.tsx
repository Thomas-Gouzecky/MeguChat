'use client';

import { Button } from './ui/button';
import { cn } from 'cn';

import { LogOut } from 'lucide-react';

import { redirect } from 'next/navigation';

export function SignOutButton({
  className,
  ...props
}: React.ComponentProps<'button'>) {
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
async function handleSignOut() {
  const response = await fetch('/api/auth/logout', {
    method: 'POST',
    credentials: 'include',
  });
  if (!response.ok) {
    alert('Sign out failed');
    return;
  }

  redirect('/login');
}
