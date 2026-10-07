import { deleteMessage } from '@/lib/api/messages';
import { removeMessage } from '@/store/slices/messagesSlice';
import { AppDispatch } from '@/store/store';
import { Button } from '@meguchat/ui/components/ui/button';
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@meguchat/ui/components/ui/dialog';
import { useDispatch } from 'react-redux';

export default function DeleteMessageButton({
  message,
  open,
  onOpenChange,
}: {
  message: MessageType;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const dispatch = useDispatch<AppDispatch>();
  async function handleSubmit() {
    const response = await deleteMessage(
      message.group_chat_id.toString(),
      message.id.toString(),
    );

    if ('detail' in response && response.status >= 400) {
      console.error('Failed to delete message:', response.detail);
      return;
    }
    dispatch(
      removeMessage({
        groupchat_id: message.group_chat_id.toString(),
        message_id: message.id.toString(),
      }),
    );
    onOpenChange(false);
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
