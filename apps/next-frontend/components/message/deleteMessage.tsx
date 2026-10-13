import { Button } from '@meguchat/ui/components/ui/button';
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@meguchat/ui/components/ui/dialog';
import { useSignalR } from '../signalr/SignalRContext';

export default function DeleteMessageButton({
  message,
  open,
  onOpenChange,
}: {
  message: MessageType;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { connection } = useSignalR();
  async function handleSubmit() {
    try {
      await connection?.invoke(
        'DeleteMessage',
        Number(message.group_chat_id),
        Number(message.id),
      );
      onOpenChange(false);
    } catch (error) {
      console.error('Error deleting message: ', error);
    }
  }
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent showCloseButton={false}>
        <DialogHeader>
          <DialogTitle>Delete Message</DialogTitle>
          <DialogDescription>
            Are you sure you want to delete this message? This action cannot be
            undone.
          </DialogDescription>
          <Button onClick={handleSubmit} variant="destructive" type="submit">
            Delete
          </Button>
          <DialogClose render={<Button variant="outline">Cancel</Button>} />
        </DialogHeader>
      </DialogContent>
    </Dialog>
  );
}
