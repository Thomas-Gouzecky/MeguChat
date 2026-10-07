'use client';

// import { MessageAnimated } from '@meguchat/ui/components/message-animated';
import { Button } from '@meguchat/ui/components/ui/button';
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@meguchat/ui/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@meguchat/ui/components/ui/dropdown-menu';
import {
  Empty,
  EmptyDescription,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
} from '@meguchat/ui/components/ui/empty';
import {
  InputGroup,
  InputGroupAddon,
  InputGroupButton,
} from '@meguchat/ui/components/ui/input-group';
import {
  MessageScroller,
  MessageScrollerButton,
  MessageScrollerContent,
  MessageScrollerProvider,
  MessageScrollerViewport,
} from '@meguchat/ui/components/ui/message-scroller';
import { RootState } from '@/store/store';
import { useSelector } from 'react-redux';
import { MessageBubble } from './MessageBubble';

export function MessagingInterface({ groupchatId }: { groupchatId: string }) {
  const messages = useSelector(
    (state: RootState) =>
      state.messages.messagesByGroupchatId[groupchatId] || [],
  );

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const messageContent = formData.get('message') as string;
    // Here you would typically dispatch an action to send the message
    console.log('Sending message:', messageContent);
  }
  return (
    <MessageScrollerProvider>
      <div className="relative flex flex-col gap-4">
        <Card className="mx-auto h-140 w-full max-w-sm gap-0">
          <CardHeader className="gap-1 border-b">
            <CardTitle>New Chat</CardTitle>
            <CardDescription>How can I help you today?</CardDescription>
          </CardHeader>
          <CardContent className="flex-1 overflow-hidden p-0">
            {/* {messages.length === 0 ? (
                    <Empty className="h-full">
                        <EmptyHeader>
                        <EmptyMedia variant="icon">
                            <MessageCircleDashedIcon />
                        </EmptyMedia>
                        <EmptyTitle>Morning, shadcn!</EmptyTitle>
                        <EmptyDescription>
                            What are we working on today? Press send to start a new
                            conversation
                        </EmptyDescription>
                        </EmptyHeader>
                    </Empty>
                    ) : ( */}
            <MessageScroller>
              <MessageScrollerViewport>
                <MessageScrollerContent className="p-(--card-spacing)">
                  {messages.map((message) => (
                    <MessageBubble key={message.id} MessageObject={message} />
                  ))}
                </MessageScrollerContent>
              </MessageScrollerViewport>
              {/* <MessageScrollerButton /> */}
            </MessageScroller>
            {/* )} */}
          </CardContent>
          <CardFooter className="flex-col gap-2">
            <form onSubmit={handleSubmit} className="w-full">
              <InputGroup>
                <div className="h-14 w-full px-3 py-2.5">
                  <span
                    className="line-clamp-2 opacity-60 data-[status=ready]:opacity-100"
                    data-status={status}
                  >
                    {status === 'ready' ? 'Type a message...' : 'Processing...'}
                  </span>
                </div>
              </InputGroup>
            </form>
          </CardFooter>
        </Card>
        <div className="px-0.5 text-center text-xs text-muted-foreground">
          Demo is read only. Press send to send messages.
        </div>
      </div>
    </MessageScrollerProvider>
  );
}
