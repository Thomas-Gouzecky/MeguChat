import { DisplayCreateGroupchatForm } from '@/components/chats/createForm/displayCreateGroupchatForm';
import Groupchats from '@/components/chats/GroupChats';

export default function GroupChatsPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background">
      <DisplayCreateGroupchatForm />
      <Groupchats className="mx-auto w-full max-w-sm" />
    </div>
  );
}
