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
import { Trash } from 'lucide-react';
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
  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const response = await deleteMessage(
      message.group_chat_id.toString(),
      message.id.toString(),
    );

    if ('detail' in response && response.status >= 400) {
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
      <form onSubmit={handleSubmit}>
        <DialogContent showCloseButton={false}>
          <DialogHeader>
            <DialogTitle>Delete Message</DialogTitle>
            <DialogDescription>
              Are you sure you want to delete this message? This action cannot
              be undone.
            </DialogDescription>
            <DialogClose render={<Button variant="outline">No</Button>} />
            <Button variant="destructive" type="submit">
              <Trash />
              Yes
            </Button>
          </DialogHeader>
        </DialogContent>
      </form>
    </Dialog>
  );
}
