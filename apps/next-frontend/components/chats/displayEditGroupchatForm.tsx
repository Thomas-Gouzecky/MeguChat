'use client';

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@meguchat/ui/components/ui/dialog';
import { useState } from 'react';
import { EditGroupchatForm } from './EditGroupchat-form';
import { Button } from '@meguchat/ui/components/ui/button';
import { Pencil } from 'lucide-react';

export default function DisplayCreateGroupchatForm({
  groupchat,
}: {
  groupchat: Groupchat;
}) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={
          <Button variant="outline">
            <Pencil />
          </Button>
        }
      >
        Edit Group Chat
      </DialogTrigger>

      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Edit Group Chat</DialogTitle>
          <DialogDescription>
            Edit the group chat by entering a new name and selecting users to
            add to the chat.
          </DialogDescription>
        </DialogHeader>

        <EditGroupchatForm groupchat={groupchat} className="px-4" />
      </DialogContent>
    </Dialog>
  );
}
