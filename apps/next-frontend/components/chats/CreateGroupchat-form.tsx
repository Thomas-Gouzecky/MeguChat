'use client';

import { Button } from '@meguchat/ui/components/ui/button';
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
} from '@meguchat/ui/components/ui/field';
import { Input } from '@meguchat/ui/components/ui/input';
import { cn } from '@meguchat/ui/lib/utils';
import { useState } from 'react';
import { ScrollArea, ScrollBar } from '@meguchat/ui/components/ui/scroll-area';
import { createGroupchat } from '@/lib/api/groupchats';
import { addGroupchat } from '@/store/slices/groupchatSlice';
import { AppDispatch } from '@/store/store';

import { useRouter } from 'next/navigation';
import { useDispatch } from 'react-redux';
import { DisplayAllUsers } from './users/displayAllUsers';
import { Label } from '@meguchat/ui/components/ui/label';

export function CreateGroupchatForm({
  className,
  ...props
}: React.ComponentProps<'div'>) {
  const [error, setError] = useState<ErrorResponse | null>(null);
  const router = useRouter();
  const dispatch = useDispatch<AppDispatch>();

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    const formData = new FormData(event.currentTarget);
    const groupchatName = formData.get('groupchat-name') as string;
    const selectedUsers = formData.getAll('users') as string[];

    const response = await createGroupchat({
      name: groupchatName,
      user_ids: selectedUsers,
    });

    // would be an error
    if ('status' in response && response.status >= 400) {
      setError(response);
      return;
    }

    // would be a successful groupchat creation
    if ('groupchat_id' in response) {
      dispatch(addGroupchat(response));
      router.push(`/chats/${response.groupchat_id}`);
    }
  }
  return (
    <div className={cn('flex flex-col gap-6', className)} {...props}>
      <form onSubmit={handleSubmit}>
        {error && (
          <FieldDescription className="text-destructive">
            {error.title}: {error.detail}
          </FieldDescription>
        )}
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="groupchat-name">Group Chat Name</FieldLabel>
            <Input
              id="groupchat-name"
              name="groupchat-name"
              type="text"
              placeholder="Your group chat name"
            />
          </Field>
          <Field>
            <Label htmlFor="users">Users</Label>
            <ScrollArea className="h-32 w-full">
              <div className="pr-4 pt-2">
                <DisplayAllUsers />
              </div>
            </ScrollArea>
          </Field>
          <Field>
            <Button type="submit">Create Group Chat</Button>
          </Field>
        </FieldGroup>
      </form>
    </div>
  );
}
