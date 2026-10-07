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
  MessageFooter,
} from '@meguchat/ui/components/ui/message';
import { useSelector } from 'react-redux';
import { JSX } from 'react/jsx-runtime';
import { MessageDropDownMenu } from './MessageDropDown';

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
  const lastMessage = messageGroup.at(-1) ?? firstMessage;
  const status = lastMessage.status ?? 'sent';

  const messageResponses: Record<'sending' | 'sent' | 'error', JSX.Element> = {
    sending: <span className="font-normal">Sending...</span>,
    sent: (
      <span className="font-normal">
        Sent{' '}
        {new Date(lastMessage.created_at).toLocaleTimeString('en-US', {
          hour: 'numeric',
          minute: '2-digit',
        })}
      </span>
    ),
    error: (
      <span className="font-normal text-destructive">
        Failed to send message
      </span>
    ),
  };
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
        <BubbleGroup className="w-full">
          {messageGroup.map((message) => (
            <div
              key={message.id}
              className={`group/message-row flex items-center gap-2 ${
                alignment === 'end' ? 'flex-row-reverse' : ''
              }`}
            >
              <Bubble>
                <BubbleContent>{message.content}</BubbleContent>
              </Bubble>
              <div className="pointer-events-none opacity-0 transition-opacity group-hover/message-row:pointer-events-auto group-hover/message-row:opacity-100 group-focus-within/message-row:pointer-events-auto group-focus-within/message-row:opacity-100">
                <MessageDropDownMenu message={message} />
              </div>
            </div>
          ))}
          <MessageFooter className="gap-2">
            {messageResponses[status]}
          </MessageFooter>
        </BubbleGroup>
      </MessageContent>
    </Message>
  );
}
