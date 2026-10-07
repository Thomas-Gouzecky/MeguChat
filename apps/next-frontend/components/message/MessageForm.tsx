'use client';

import { ArrowUp } from 'lucide-react';

import { createNewMessage } from '@/lib/api/messages';
import { addMessage } from '@/store/slices/messagesSlice';
import { AppDispatch } from '@/store/store';
import { Input } from '@meguchat/ui/components/ui/input';
import {
  InputGroup,
  InputGroupAddon,
  InputGroupButton,
} from '@meguchat/ui/components/ui/input-group';
import { useDispatch } from 'react-redux';

export default function MessageForm({ groupchatId }: { groupchatId: string }) {
  const dispatch = useDispatch<AppDispatch>();

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const messageContent = formData.get('message') as string;
    const response = await createNewMessage(groupchatId, messageContent);

    if ('status' in response && response.status >= 400) {
      console.error('Error creating message:', response);
      return;
    }

    if ('id' in response) {
      dispatch(
        addMessage({
          groupchat_id: groupchatId,
          message: response,
        }),
      );
    }
  }
  return (
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
  );
}
