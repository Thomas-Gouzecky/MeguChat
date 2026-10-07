'use client';

// import { MessageAnimated } from '@meguchat/ui/components/message-animated';
import { Button } from '@meguchat/ui/components/ui/button';
import { ArrowUp } from 'lucide-react';
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
import { AppDispatch, RootState } from '@/store/store';
import { useDispatch, useSelector } from 'react-redux';
import { MessageBubble } from './MessageBubble';
import { Input } from '@meguchat/ui/components/ui/input';
import { useEffect } from 'react';
import { fetchMessagesByGroupchatId } from '@/store/thunks/messagesThunk';
import { fetchGroupchatById } from '@/store/thunks/groupchatThunk';

export function MessagingInterface({ groupchatId }: { groupchatId: string }) {
  const dispatch = useDispatch<AppDispatch>();
  const groupchat = useSelector((state: RootState) =>
    state.groupchats.groupchats.find(
      (chat) => chat.groupchat_id.toString() === groupchatId,
    ),
  );
  const { messagesByGroupchatId, loadingByGroupchatId, errorByGroupchatId } =
    useSelector((state: RootState) => state.messages);

  const messages = messagesByGroupchatId[Number(groupchatId)] ?? [];
  const isLoading = loadingByGroupchatId[Number(groupchatId)] ?? false;
  const error = errorByGroupchatId[Number(groupchatId)] ?? null;

  useEffect(() => {
    // Fetch messages for the groupchat when the component mounts
    dispatch(fetchMessagesByGroupchatId(Number(groupchatId)));
    dispatch(fetchGroupchatById(groupchatId)); // Fetch groupchat details
  }, [dispatch, groupchatId]);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const messageContent = formData.get('message') as string;
    // Here you would typically dispatch an action to send the message
    console.log('Sending message:', messageContent);
  }

  if (isLoading) {
    return (
      <div className="p-4 text-sm text-muted-foreground">
        Loading messages...
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 text-sm text-destructive flex justify-center items-center flex-col gap-2">
        <div>
          Error {error.status}: {error.title}
        </div>
        <div>{error.detail}</div>
      </div>
    );
  }

  return (
    <MessageScrollerProvider>
      <div className="relative flex flex-col gap-4">
        <Card className="mx-auto h-140 w-full max-w-sm gap-0">
          <CardHeader className="gap-1 border-b">
            <CardTitle>{groupchat?.name || 'New Chat'}</CardTitle>
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
              <MessageScrollerButton />
            </MessageScroller>
            {/* )} */}
          </CardContent>
          <CardFooter className="flex-col gap-2">
            <form onSubmit={handleSubmit} className="w-full">
              <InputGroup>
                <InputGroupAddon>
                  <Input
                    id="message"
                    name="message"
                    type="text"
                    placeholder="Type a message..."
                  />
                  <InputGroupButton
                    type="submit"
                    variant="default"
                    size="icon-sm"
                    className="ml-auto"
                  >
                    <ArrowUp />
                    <span className="sr-only">Send</span>
                  </InputGroupButton>
                </InputGroupAddon>
              </InputGroup>
            </form>
          </CardFooter>
        </Card>
      </div>
    </MessageScrollerProvider>
  );
}
