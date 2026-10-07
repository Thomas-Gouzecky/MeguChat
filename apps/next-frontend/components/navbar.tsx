'use client';

import { SignOutButton } from './auth/signout-button';
import { ModeToggle } from '@meguchat/ui/components/mode-toggle';
import { useSelector } from 'react-redux';
import { RootState } from '@/store/store';
import { Separator } from '@meguchat/ui/components/ui/separator';

export default function Navbar() {
  const user = useSelector((state: RootState) => state.auth.user);
  return (
    <nav className="p-4 border-b light:border-accent/20 dark:border-accent/50">
      <div className="container mx-auto flex items-center justify-between">
        <div className="dark:text-white font-bold text-lg light:text-accent">
          MeguChat
        </div>
        <div className="flex items-center space-x-4">
          <ModeToggle />
          {user && (
            <>
              <Separator orientation="vertical" />
              <div className="flex items-center gap-2 dark:text-white font-bold text-md light:text-accent">
                <span>{user.username}</span>
                <SignOutButton />
              </div>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
