'use client';

import { deleteGroupchat } from '@/lib/api/groupchats';

import { Button } from '@meguchat/ui/components/ui/button';
import { Trash } from 'lucide-react';
import { useDispatch } from 'react-redux';

export default function DeleteGroupchatButton({
  groupchatId,
}: {
  groupchatId: number;
}) {
  const dispatch = useDispatch();
  async function handleDelete(event: React.MouseEvent<HTMLButtonElement>) {
    event.preventDefault();

    // Deletes from the database
    const response = await deleteGroupchat(groupchatId);

    if ('status' in response && response.status >= 400) {
      alert('Failed to delete group chat');
      return;
    }

    // Deletes from the store -> should automatically update the UI
  }

  return (
    <Button
      onClick={handleDelete}
      variant="outline"
      type="button"
      className="text-destructive hover:text-destructive"
    >
      <Trash />
    </Button>
  );
}
