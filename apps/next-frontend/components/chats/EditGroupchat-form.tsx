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

import { useRouter } from 'next/navigation';
import { editGroupchat } from '@/lib/api/groupchats';
import { updateGroupchat } from '@/store/slices/groupchatSlice';
import { AppDispatch } from '@/store/store';
import { useDispatch } from 'react-redux';

export function EditGroupchatForm({
  groupchat,
  setOpen,
  className,
  ...props
}: React.ComponentProps<'div'> & {
  groupchat: Groupchat;
  setOpen: (open: boolean) => void;
}) {
  const [error, setError] = useState<ErrorResponse | null>(null);
  const [groupchatState, setGroupchatState] = useState<Groupchat>(groupchat);
  const dispatch = useDispatch<AppDispatch>();

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    const formData = new FormData(event.currentTarget);
    const groupchatName = formData.get('groupchat-name') as string;
    const selectedUsers = formData.getAll('users') as string[];

    setGroupchatState({
      ...groupchatState,
      name: groupchatName,
    });

    const response = await editGroupchat(groupchat.groupchat_id, {
      name: groupchatName,
      users: selectedUsers,
    });

    // would be an error
    if ('status' in response && response.status >= 400) {
      setError(response);
      return;
    }

    // would be a successful groupchat creation
    if ('groupchat_id' in response) {
      dispatch(updateGroupchat(response));
      setOpen(false);
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
            <FieldLabel htmlFor="groupchat-name">
              Edit Group Chat Name
            </FieldLabel>
            <Input
              id="groupchat-name"
              name="groupchat-name"
              type="text"
              placeholder="Your group chat name"
              value={groupchatState.name}
              onChange={(event) =>
                setGroupchatState({
                  ...groupchatState,
                  name: event.target.value,
                })
              }
            />
          </Field>
          <Field>
            <div className="flex items-center">
              <FieldLabel htmlFor="users">Users</FieldLabel>
              <ScrollArea className="h-32 w-full">
                <ScrollBar orientation="vertical" />
                {/* Get all the users from the /api/users endpoint */}
                {/* Display each user in the scroll area and allow selection */}
              </ScrollArea>
            </div>
          </Field>
          <Field>
            <Button type="submit">Edit Group Chat</Button>
          </Field>
        </FieldGroup>
      </form>
    </div>
  );
}
