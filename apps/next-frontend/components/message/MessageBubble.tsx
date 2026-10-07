import { RootState } from '@/store/store';
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from '@meguchat/ui/components/ui/avatar';
import {
  Bubble,
  BubbleContent,
  BubbleGroup,
} from '@meguchat/ui/components/ui/bubble';
import {
  Message,
  MessageAvatar,
  MessageContent,
} from '@meguchat/ui/components/ui/message';
import { useSelector } from 'react-redux';

export function MessageBubble({
  messageGroup,
}: {
  messageGroup: MessageType[];
}) {
  const user_id = useSelector((state: RootState) => state.auth.user?.user_id);
  const firstMessage = messageGroup[0];

  if (!firstMessage) {
    return null;
  }

  const alignment = firstMessage.user_id === user_id ? 'end' : 'start';
  return (
    <Message align={alignment}>
      <MessageAvatar>
        <Avatar>
          <AvatarImage src="/avatars/10.png" alt="@me" />
          <AvatarFallback>
            {firstMessage.user_id?.substring(0, 2).toUpperCase() || 'ME'}
          </AvatarFallback>
        </Avatar>
      </MessageAvatar>
      <MessageContent>
        <BubbleGroup>
          {messageGroup.map((message) => (
            <Bubble key={message.id}>
              <BubbleContent>{message.content}</BubbleContent>
            </Bubble>
          ))}
        </BubbleGroup>
      </MessageContent>
    </Message>
  );
}
