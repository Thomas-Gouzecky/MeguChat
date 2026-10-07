'use client';

import { PencilIcon, ShareIcon, TrashIcon } from 'lucide-react';

import { Button } from '@meguchat/ui/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@meguchat/ui/components/ui/dropdown-menu';
import DeleteMessageButton from './deleteMessage';

export function MessageDropDownMenu({
  groupchat_id,
  message_id,
}: {
  groupchat_id: string;
  message_id: string;
}) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        render={<Button variant="outline">Actions</Button>}
      />
      <DropdownMenuContent>
        <DropdownMenuGroup>
          <DropdownMenuItem>
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
          <DropdownMenuItem variant="destructive">
            <DeleteMessageButton
              groupchat_id={groupchat_id}
              message_id={message_id}
            />
          </DropdownMenuItem>
        </DropdownMenuGroup>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
