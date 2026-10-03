'use client';

import { Moon, Sun } from 'lucide-react';
import { useTheme } from 'next-themes';

import { Button } from './ui/button';
import { cn } from 'cn';

export function ModeToggle({
  className,
  ...props
}: React.ComponentProps<'button'>) {
  const { resolvedTheme, setTheme } = useTheme();

  return (
    <div className={cn(className)}>
      <Button
        aria-label="Toggle theme"
        className="transition-none active:translate-y-0!"
        size="icon"
        variant="ghost"
        onClick={() => setTheme(resolvedTheme === 'dark' ? 'light' : 'dark')}
      >
        <Sun className="h-4 w-4 rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
        <Moon className="absolute h-4 w-4 rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
      </Button>
    </div>
  );
}
