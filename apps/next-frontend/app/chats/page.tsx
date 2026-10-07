import Groupchats from '@/components/chats/Groupchats';
import { DisplayCreateGroupchatForm } from '@/components/chats/createForm/displayCreateGroupchatForm';

export default function GroupChatsPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background">
      <DisplayCreateGroupchatForm />
      <Groupchats className="mx-auto w-full max-w-sm" />
    </div>
  );
}
