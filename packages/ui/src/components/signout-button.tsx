'use client';

import { Button } from './ui/button';
import { cn } from 'cn';

import { LogOut } from 'lucide-react';

export function SignOutButton({
  className,
  ...props
}: React.ComponentProps<'button'>) {
  return (
    <div className={cn('flex flex-col gap-6', className)}>
      <Button
        type="button"
        variant="ghost"
        className={cn('w-full justify-start', className)}
        {...props}
      >
        <LogOut className="h-4 w-4" />
        Sign Out
      </Button>
    </div>
  );
}
