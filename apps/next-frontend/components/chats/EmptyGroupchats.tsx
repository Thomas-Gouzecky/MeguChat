import { MessagesCircle } from 'lucide-react';

import {
  Empty,
  EmptyContent,
  EmptyDescription,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
} from '@meguchat/ui/components/ui/empty';
import { DisplayCreateGroupchatForm } from '@/components/chats/displayCreateGroupchatForm';

export function EmptyGroupchats() {
  return (
    <Empty className="h-full bg-muted/30">
      <EmptyHeader>
        <EmptyMedia variant="icon">
          <MessagesCircle />
        </EmptyMedia>
        <EmptyTitle>No Group Chats</EmptyTitle>
        <EmptyDescription className="max-w-xs text-pretty">
          You have no group chats yet. Click the button below to create a new
          group chat.
        </EmptyDescription>
      </EmptyHeader>
      <EmptyContent>
        <DisplayCreateGroupchatForm />
      </EmptyContent>
    </Empty>
  );
}
