'use client';

import { ArrowUp } from 'lucide-react';
import { HubConnectionState } from '@microsoft/signalr';

import {
  InputGroup,
  InputGroupAddon,
  InputGroupButton,
  InputGroupTextarea,
} from '@meguchat/ui/components/ui/input-group';
import { useSignalR } from '../signalr/SignalRContext';
import { addMessage, replaceMessage } from '@/store/slices/messagesSlice';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/store/store';

export default function MessageForm({ groupchatId }: { groupchatId: string }) {
  const { connection } = useSignalR();
  const dispatch = useDispatch<AppDispatch>();
  const userId = useSelector((state: RootState) => state.auth.user?.user_id);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const messageContent = (formData.get('message') as string | null)?.trim();

    if (!messageContent) {
      return;
    }

    event.currentTarget.reset();
    const textarea = event.currentTarget.elements.namedItem(
      'message',
    ) as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.overflowY = 'hidden';

    // create a temporary message object to display in the UI while waiting for the server response
    const temporaryId = Date.now();

    const temporaryMessage: MessageType = {
      id: temporaryId,
      content: messageContent,
      user_id: userId ?? 'temp-user',
      group_chat_id: Number(groupchatId),
      created_at: new Date().toISOString(),
      modified_at: new Date().toISOString(),
      status: 'sending',
    };

    dispatch(
      addMessage({
        groupchat_id: groupchatId,
        message: temporaryMessage,
      }),
    );

    // updates the message status to 'error' if the connection is not established
    if (!connection || connection.state !== HubConnectionState.Connected) {
      dispatch(
        replaceMessage({
          groupchat_id: groupchatId,
          message_id: temporaryId,
          message: { ...temporaryMessage, status: 'error' },
        }),
      );
      return;
    }

    try {
      const response = await connection.invoke<MessageType>(
        'SendMessage',
        Number(groupchatId),
        {
          content: messageContent,
        },
      );

      // replace the temporary message with the actual message from the server
      dispatch(
        replaceMessage({
          groupchat_id: groupchatId,
          message_id: temporaryId,
          message: { ...response, status: 'sent' },
        }),
      );
    } catch (error) {
      console.error('Error sending message: ', error);
      // update the temporary message status to 'error' to indicate that the message failed to send
      dispatch(
        replaceMessage({
          groupchat_id: groupchatId,
          message_id: temporaryId,
          message: { ...temporaryMessage, status: 'error' },
        }),
      );
    }
  }
  return (
    <form onSubmit={handleSubmit} className="w-full">
      <InputGroup className="h-auto min-h-8 items-center">
        <InputGroupAddon className="w-full min-w-0 items-end">
          <InputGroupTextarea
            id="message"
            name="message"
            placeholder="Type a message..."
            rows={1}
            className="min-h-8 min-w-0 basis-0 resize-none overflow-y-hidden field-sizing-fixed text-primary"
            onInput={(event) => {
              const textarea = event.currentTarget;
              textarea.style.height = 'auto';
              textarea.style.height = `${Math.min(textarea.scrollHeight, 128)}px`;
              textarea.style.overflowY =
                textarea.scrollHeight > 128 ? 'auto' : 'hidden';
            }}
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
  );
}
