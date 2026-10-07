'use client';

import { ArrowUp } from 'lucide-react';

import { createNewMessage } from '@/lib/api/messages';
import {
  addMessage,
  editMessage,
  replaceMessage,
} from '@/store/slices/messagesSlice';
import { AppDispatch, RootState } from '@/store/store';
import {
  InputGroup,
  InputGroupAddon,
  InputGroupButton,
  InputGroupTextarea,
} from '@meguchat/ui/components/ui/input-group';
import { useDispatch, useSelector } from 'react-redux';

export default function MessageForm({ groupchatId }: { groupchatId: string }) {
  const dispatch = useDispatch<AppDispatch>();
  const userId = useSelector((state: RootState) => state.auth.user?.user_id);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const messageContent = formData.get('message') as string;
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
    event.currentTarget.reset();
    const textarea = event.currentTarget.elements.namedItem(
      'message',
    ) as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.overflowY = 'hidden';

    try {
      const response = await createNewMessage(groupchatId, messageContent);

      if (!('id' in response)) {
        dispatch(
          editMessage({
            groupchat_id: groupchatId,
            message: { ...temporaryMessage, status: 'error' },
          }),
        );
        return;
      }

      dispatch(
        replaceMessage({
          groupchat_id: groupchatId,
          message_id: temporaryId,
          message: { ...response, status: 'sent' },
        }),
      );
    } catch {
      dispatch(
        editMessage({
          groupchat_id: groupchatId,
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
