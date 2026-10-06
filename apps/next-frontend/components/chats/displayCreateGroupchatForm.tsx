'use client';

import { CreateGroupchatForm } from '@/components/chats/CreateGroupchat-form';
import { Button } from '@meguchat/ui/components/ui/button';
import {
  Drawer,
  DrawerClose,
  DrawerContent,
  DrawerDescription,
  DrawerFooter,
  DrawerHeader,
  DrawerTitle,
  DrawerTrigger,
} from '@meguchat/ui/components/ui/drawer';

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@meguchat/ui/components/ui/dialog';
import { useState } from 'react';
import { useMediaQuery } from 'usehooks-ts';

export function DisplayCreateGroupchatForm() {
  const [open, setOpen] = useState(false);
  const isDesktop = useMediaQuery('(min-width: 768px)', {
    initializeWithValue: false,
  });

  if (isDesktop) {
    return (
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogTrigger render={<Button variant="outline" />}>
          Create Group Chat
        </DialogTrigger>

        <DialogContent className="sm:max-w-[425px]">
          <DialogHeader>
            <DialogTitle>Create Group Chat</DialogTitle>
            <DialogDescription>
              Create a new group chat by entering a name and selecting users to
              add to the chat.
            </DialogDescription>
          </DialogHeader>

          <CreateGroupchatForm className="px-4" />
        </DialogContent>
      </Dialog>
    );
  }
  return (
    <Drawer open={open} onOpenChange={setOpen}>
      <DrawerTrigger render={<Button variant="outline" />}>
        Create Group Chat
      </DrawerTrigger>
      <DrawerContent>
        <DrawerHeader className="text-left">
          <DrawerTitle>Create Group Chat</DrawerTitle>
          <DrawerDescription>
            Create a new group chat by entering a name and selecting users to
            add to the chat.
          </DrawerDescription>
        </DrawerHeader>
        <CreateGroupchatForm className="px-4" />
        <DrawerFooter className="pt-2">
          <DrawerClose render={<Button variant="outline" />}>
            Cancel
          </DrawerClose>
        </DrawerFooter>
      </DrawerContent>
    </Drawer>
  );
}
