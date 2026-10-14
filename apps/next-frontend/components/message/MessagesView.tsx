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
import { Marker, MarkerContent } from '@meguchat/ui/components/ui/marker';

export default function MessagesView({ groupchatId }: { groupchatId: string }) {
  const dispatch = useDispatch<AppDispatch>();
  const { messagesByGroupchatId } = useSelector(
    (state: RootState) => state.messages,
  );
  const { typingUsersByGroupchatId } = useSelector(
    (state: RootState) => state.groupchatById,
  );
  const usersTyping = typingUsersByGroupchatId[groupchatId] ?? [];

  const messages = messagesByGroupchatId[Number(groupchatId)] ?? [];
  const { scrollToEnd } = useMessageScroller();
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
    scrollToEnd({ behavior: 'smooth' });
  }, [messages.length, scrollToEnd]);

  return (
    <MessageScroller>
      <MessageScrollerViewport>
        <MessageScrollerContent className="p-(--card-spacing)">
          {messageGroups.map((messageGroup) => (
            <MessageScrollerItem
              key={messageGroup[0].id}
              messageId={messageGroup[0].id.toString()}
            >
              <MessageBubble messageGroup={messageGroup} />
            </MessageScrollerItem>
          ))}
          <Marker role="status">
            <MarkerContent className="shimmer">
              {usersTyping.length > 0 && (
                <span className="font-medium">
                  {usersTyping.join(', ')}{' '}
                  {usersTyping.length === 1 ? 'is' : 'are'} typing...
                </span>
              )}
            </MarkerContent>
          </Marker>
        </MessageScrollerContent>
      </MessageScrollerViewport>
      <MessageScrollerButton />
    </MessageScroller>
  );
}
