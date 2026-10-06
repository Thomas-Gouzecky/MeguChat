'use client';

import { Button } from '@meguchat/ui/components/ui/button';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@meguchat/ui/components/ui/card';
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

import { useRouter } from 'next/navigation';

export function CreateGroupchatForm({
  className,
  ...props
}: React.ComponentProps<'div'>) {
  const [error, setError] = useState<ErrorResponse | null>(null);
  const router = useRouter();

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    const formData = new FormData(event.currentTarget);
    const groupchatName = formData.get('groupchat-name') as string;
    const selectedUsers = formData.getAll('users') as string[];

    const response = await createGroupchat({
      name: groupchatName,
      users: selectedUsers,
    });

    if ('status' in response && response.status >= 400) {
      setError(response);
      return;
    }

    router.refresh();
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
            <Button type="submit">Create Group Chat</Button>
          </Field>
        </FieldGroup>
      </form>
    </div>
  );
}
