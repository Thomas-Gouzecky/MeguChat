'use client';

import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@meguchat/ui/components/ui/card';
import { MessageScrollerProvider } from '@meguchat/ui/components/ui/message-scroller';
import { AppDispatch, RootState } from '@/store/store';
import { useDispatch, useSelector } from 'react-redux';
import { useEffect } from 'react';
import { fetchGroupchatById } from '@/store/thunks/groupchatThunk';
import MessageForm from './MessageForm';
import MessagesView from './MessagesView';

export function MessagingInterface({ groupchatId }: { groupchatId: string }) {
  const dispatch = useDispatch<AppDispatch>();
  const { groupchatsByGroupchatId: groupchatById } = useSelector(
    (state: RootState) => state.groupchatById,
  );

  const groupchat = groupchatById[groupchatId];

  useEffect(() => {
    dispatch(fetchGroupchatById(groupchatId));
  }, [dispatch, groupchatId]);

  return (
    <MessageScrollerProvider autoScroll>
      <div className="relative flex flex-col gap-4">
        <Card className="mx-auto h-140 w-full max-w-sm gap-0">
          <CardHeader className="gap-1 border-b">
            <CardTitle>{groupchat?.name || 'New Chat'}</CardTitle>
          </CardHeader>
          <CardContent className="flex-1 overflow-hidden p-0">
            <MessagesView groupchatId={groupchatId} />
          </CardContent>
          <CardFooter className="flex-col gap-2">
            <MessageForm groupchatId={groupchatId} />
          </CardFooter>
        </Card>
      </div>
    </MessageScrollerProvider>
  );
}
