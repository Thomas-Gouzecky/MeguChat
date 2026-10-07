import { AppDispatch, RootState } from '@/store/store';
import { fetchMessagesByGroupchatId } from '@/store/thunks/messagesThunk';
import {
  MessageScroller,
  MessageScrollerButton,
  MessageScrollerContent,
  MessageScrollerViewport,
} from '@meguchat/ui/components/ui/message-scroller';
import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { MessageBubble } from './MessageBubble';

export default function MessagesView({ groupchatId }: { groupchatId: string }) {
  const dispatch = useDispatch<AppDispatch>();
  const { messagesByGroupchatId, loadingByGroupchatId, errorByGroupchatId } =
    useSelector((state: RootState) => state.messages);

  const messages = messagesByGroupchatId[Number(groupchatId)] ?? [];
  const isLoading = loadingByGroupchatId[Number(groupchatId)] ?? false;
  const error = errorByGroupchatId[Number(groupchatId)] ?? null;
  const messageGroups = messages.reduce<MessageType[][]>((groups, message) => {
    const previousGroup = groups.at(-1);

    if (previousGroup?.[0]?.user_id === message.user_id) {
      previousGroup.push(message);
    } else {
      groups.push([message]);
    }

    return groups;
  }, []);

  useEffect(() => {
    dispatch(fetchMessagesByGroupchatId(Number(groupchatId)));
  }, [dispatch, groupchatId]);
  return (
    <MessageScroller>
      <MessageScrollerViewport>
        <MessageScrollerContent className="p-(--card-spacing)">
          {messageGroups.map((messageGroup) => (
            <MessageBubble
              key={messageGroup[0].id}
              messageGroup={messageGroup}
            />
          ))}
        </MessageScrollerContent>
      </MessageScrollerViewport>
      <MessageScrollerButton />
    </MessageScroller>
  );
}
