import { SignOutButton } from './signout-button';
import { ModeToggle } from '@meguchat/ui/components/mode-toggle';

export default function Navbar() {
  return (
    <nav className="bg-gray-800 p-4">
      <div className="container mx-auto flex items-center justify-between">
        <div className="text-white font-bold text-lg">MeguChat</div>
        <div className="flex items-center space-x-4">
          <ModeToggle />
          <SignOutButton />
        </div>
      </div>
    </nav>
  );
}
