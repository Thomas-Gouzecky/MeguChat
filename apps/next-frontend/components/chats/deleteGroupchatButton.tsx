'use client';

import { deleteGroupchat } from '@/lib/api/groupchats';

import { Button } from '@meguchat/ui/components/ui/button';
import { Trash } from 'lucide-react';
import { useDispatch } from 'react-redux';
import { removeGroupchat } from '@/store/slices/groupchatSlice';
import { AppDispatch } from '@/store/store';

export default function DeleteGroupchatButton({
  groupchatId,
}: {
  groupchatId: number;
}) {
  const dispatch = useDispatch<AppDispatch>();
  async function handleDelete() {
    // Deletes from the database
    const response = await deleteGroupchat(groupchatId);

    if ('status' in response && response.status >= 400) {
      alert('Failed to delete group chat');
      return;
    }

    dispatch(removeGroupchat(groupchatId));
  }

  return (
    <Button
      onClick={handleDelete}
      variant="outline"
      type="button"
      className="cursor-pointer text-destructive hover:text-destructive"
    >
      <Trash />
    </Button>
  );
}
