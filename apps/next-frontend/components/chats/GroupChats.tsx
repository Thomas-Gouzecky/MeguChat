'use client';

import { useEffect, useState } from 'react';

import { EmptyGroupchats } from '@/components/chats/EmptyGroupchats';
import { getUsersGroupchats } from '@/lib/api/groupchats';
import { cn } from '@meguchat/ui/lib/utils';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@meguchat/ui/components/ui/card';
import DeleteGroupchatButton from './deleteGroupchatButton';
import DisplayEditGroupchatForm from './displayEditGroupchatForm';

export default function Groupchats({ className }: { className?: string }) {
  const [groupchats, setGroupchats] = useState<Groupchat[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let isMounted = true;

    async function fetchGroupchats() {
      const response = await getUsersGroupchats();

      if (!isMounted) {
        return;
      }

      if (Array.isArray(response)) {
        setGroupchats(response);
      } else {
        console.error('Error fetching group chats:', response.detail);
      }

      setIsLoading(false);
    }

    fetchGroupchats();

    return () => {
      isMounted = false;
    };
  }, []);

  // Loading state
  if (isLoading) {
    return (
      <div className="p-4 text-sm text-muted-foreground">
        Loading group chats...
      </div>
    );
  }

  // User has no group chats
  if (groupchats.length === 0) {
    return <EmptyGroupchats />;
  }

  // Render the actual group chats
  return (
    <div className={cn('space-y-3 p-4', className)}>
      {groupchats.map((groupchat) => (
        <Card key={groupchat.groupchat_id}>
          <CardHeader>
            <CardTitle>{groupchat.name}</CardTitle>
            <CardDescription>
              Group chat ID: {groupchat.groupchat_id}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <CardDescription>
              Created at: {groupchat.created_at ?? 'Unknown'}
            </CardDescription>
            <DisplayEditGroupchatForm groupchat={groupchat} />
            <DeleteGroupchatButton groupchatId={groupchat.groupchat_id} />
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
