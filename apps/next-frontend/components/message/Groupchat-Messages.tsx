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
import { createNewMessage } from '@/lib/api/messages';

export function MessagingInterface({ groupchatId }: { groupchatId: string }) {
  const dispatch = useDispatch<AppDispatch>();
  const { groupchatsByGroupchatId: groupchatById } = useSelector(
    (state: RootState) => state.groupchatById,
  );
  const { messagesByGroupchatId, loadingByGroupchatId, errorByGroupchatId } =
    useSelector((state: RootState) => state.messages);

  const groupchat = groupchatById[groupchatId];
  const messages = messagesByGroupchatId[Number(groupchatId)] ?? [];
  const isLoading = loadingByGroupchatId[Number(groupchatId)] ?? false;
  const error = errorByGroupchatId[Number(groupchatId)] ?? null;

  useEffect(() => {
    dispatch(fetchMessagesByGroupchatId(Number(groupchatId)));
    dispatch(fetchGroupchatById(groupchatId));
  }, [dispatch, groupchatId]);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const messageContent = formData.get('message') as string;
    const response = await createNewMessage(groupchatId, {
      content: messageContent,
    });
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
