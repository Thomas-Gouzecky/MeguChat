import { Button } from '@meguchat/ui/components/ui/button';
import { Trash } from 'lucide-react';

export default function DeleteGroupchatButton({
  groupchatId,
}: {
  groupchatId: number;
}) {
  const handleDelete = async () => {
    try {
      const response = await fetch(`/api/groupchats/${groupchatId}`, {
        method: 'DELETE',
      });
      if (!response.ok) {
        throw new Error('Failed to delete group chat');
      }
    } catch (error) {
      console.error('Error deleting group chat:', error);
    }
  };

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
