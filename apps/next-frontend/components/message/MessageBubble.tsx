import { AppDispatch, RootState } from '@/store/store';
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from '@meguchat/ui/components/ui/avatar';
import { Bubble, BubbleContent } from '@meguchat/ui/components/ui/bubble';
import {
  Message,
  MessageAvatar,
  MessageContent,
} from '@meguchat/ui/components/ui/message';
import { useSelector } from 'react-redux';

export function MessageBubble({
  MessageObject,
}: {
  MessageObject: MessageType;
}) {
  // get the user id and then get the user object from the store
  // const user = getUserById(MessageObject.user_id);
  const user_id = useSelector((state: RootState) => state.auth.user?.user_id);
  const alignment = MessageObject.user_id === user_id ? 'end' : 'start';
  return (
    <Message align={alignment}>
      <MessageAvatar>
        <Avatar>
          <AvatarImage src="/avatars/10.png" alt="@me" />
          <AvatarFallback>
            {user_id?.substring(0, 2).toUpperCase() || 'ME'}
          </AvatarFallback>
        </Avatar>
      </MessageAvatar>
      <MessageContent>
        <Bubble>
          <BubbleContent>{MessageObject.content}</BubbleContent>
        </Bubble>
      </MessageContent>
    </Message>
  );
}
