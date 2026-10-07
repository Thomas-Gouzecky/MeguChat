'use client';

import { useEffect } from 'react';

import { EmptyGroupchats } from '@/components/chats/EmptyGroupchats';
import { fetchGroupchats } from '@/store/thunks/groupchatThunk';
import { AppDispatch, RootState } from '@/store/store';
import { cn } from '@meguchat/ui/lib/utils';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@meguchat/ui/components/ui/card';
import DeleteGroupchatButton from './deleteGroupchatButton';
import DisplayEditGroupchatForm from './editForm/displayEditGroupchatForm';
import { useDispatch, useSelector } from 'react-redux';
import Link from 'next/link';
import { DisplayGroupChatMembersAvatars } from './chatmembersAvatars/DisplayGroupChatMembersAvatars';

export default function Groupchats({ className }: { className?: string }) {
  const dispatch = useDispatch<AppDispatch>();
  const {
    groupchats,
    loading: isLoading,
    error,
  } = useSelector((state: RootState) => state.groupchats);

  useEffect(() => {
    void dispatch(fetchGroupchats());
  }, [dispatch]);

  if (error && error.status !== 404) {
    return (
      <div className="p-4 text-sm text-destructive">
        {error.title}: {error.detail}
      </div>
    );
  }

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
        <Card key={groupchat.groupchat_id} className="relative">
          <Link
            href={`/chats/${groupchat.groupchat_id}`}
            aria-label={`Open ${groupchat.name}`}
            className="absolute inset-0 z-10 cursor-pointer"
          />
          <div className="flex flex-row justify-between">
            <div className="pointer-events-none relative z-20 w-full">
              <CardHeader className="w-full">
                <CardTitle>{groupchat.name}</CardTitle>
              </CardHeader>
              <CardContent>
                <DisplayGroupChatMembersAvatars groupchat={groupchat} />
              </CardContent>
            </div>
            <div className="pointer-events-none z-20 flex flex-row gap-2 absolute right-2 top-2">
              <div className="pointer-events-auto h-fit">
                <DisplayEditGroupchatForm groupchat={groupchat} />
              </div>
              <div className="pointer-events-auto h-fit">
                <DeleteGroupchatButton groupchatId={groupchat.groupchat_id} />
              </div>
            </div>
          </div>
        </Card>
      ))}
    </div>
  );
}
