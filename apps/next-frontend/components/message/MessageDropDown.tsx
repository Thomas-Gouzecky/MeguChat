'use client';

import {
  EllipsisVertical,
  PencilIcon,
  ShareIcon,
  Trash,
} from 'lucide-react';

import { Button } from '@meguchat/ui/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@meguchat/ui/components/ui/dropdown-menu';
import { useState } from 'react';
import DeleteMessageButton from './deleteMessage';
import { EditableMessageBox } from './editMessage';

export function MessageDropDownMenu({ message }: { message: MessageType }) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [editableMessageBox, setEditableMessageBox] = useState(false);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger
          render={
            <Button variant="outline" size="icon">
              <EllipsisVertical />
            </Button>
          }
        />
        <DropdownMenuContent>
          <DropdownMenuGroup>
            <DropdownMenuItem
              onClick={() => {
                setEditableMessageBox(true);
              }}
            >
              <PencilIcon />
              Edit
            </DropdownMenuItem>
            <DropdownMenuItem>
              <ShareIcon />
              Share
            </DropdownMenuItem>
          </DropdownMenuGroup>
          <DropdownMenuSeparator />
          <DropdownMenuGroup>
            <DropdownMenuItem
              variant="destructive"
              onClick={() => setDeleteDialogOpen(true)}
            >
              <Trash />
              Delete
            </DropdownMenuItem>
          </DropdownMenuGroup>
        </DropdownMenuContent>
      </DropdownMenu>
      <DeleteMessageButton
        message={message}
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
      />
      <EditableMessageBox
        message={message}
        open={editableMessageBox}
        onOpenChange={setEditableMessageBox}
      />
    </>
  );
}
