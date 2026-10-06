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
import DisplayEditGroupchatForm from './displayEditGroupchatForm';
import { useDispatch, useSelector } from 'react-redux';
import Link from 'next/link';

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
        <Link
          key={groupchat.groupchat_id}
          href={`/chats/${groupchat.groupchat_id}`}
        >
          <Card>
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
        </Link>
      ))}
    </div>
  );
}
