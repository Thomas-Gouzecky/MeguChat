import { AppDispatch, RootState } from '@/store/store';
import { fetchMessagesByGroupchatId } from '@/store/thunks/messagesThunk';
import {
  MessageScroller,
  MessageScrollerButton,
  MessageScrollerContent,
  MessageScrollerItem,
  MessageScrollerViewport,
  useMessageScroller,
} from '@meguchat/ui/components/ui/message-scroller';
import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { MessageBubble } from './MessageBubble';

export default function MessagesView({ groupchatId }: { groupchatId: string }) {
  const dispatch = useDispatch<AppDispatch>();
  const { messagesByGroupchatId, loadingByGroupchatId, errorByGroupchatId } =
    useSelector((state: RootState) => state.messages);

  const messages = messagesByGroupchatId[Number(groupchatId)] ?? [];
  const { scrollToEnd } = useMessageScroller();
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

  useEffect(() => {
    scrollToEnd();
  }, [messages.length, scrollToEnd]);

  return (
    <MessageScroller>
      <MessageScrollerViewport>
        <MessageScrollerContent className="p-(--card-spacing)">
          {messageGroups.map((messageGroup, index) => (
            <MessageScrollerItem
              key={messageGroup[0].id}
              messageId={messageGroup[0].id.toString()}
              scrollAnchor={index === messageGroups.length - 1}
            >
              <MessageBubble messageGroup={messageGroup} />
            </MessageScrollerItem>
          ))}
        </MessageScrollerContent>
      </MessageScrollerViewport>
      <MessageScrollerButton />
    </MessageScroller>
  );
}
