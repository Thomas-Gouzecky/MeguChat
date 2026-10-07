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
  DialogTrigger,
} from '@meguchat/ui/components/ui/dialog';
import { Trash } from 'lucide-react';
import { useDispatch } from 'react-redux';

export default function DeleteMessageButton({
  groupchat_id,
  message_id,
}: {
  groupchat_id: string;
  message_id: string;
}) {
  const dispatch = useDispatch<AppDispatch>();
  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const response = await deleteMessage(groupchat_id, message_id);

    if ('detail' in response && response.status >= 400) {
      return;
    }
    dispatch(removeMessage({ groupchat_id, message_id }));
  }
  return (
    <Dialog>
      <form onSubmit={handleSubmit}>
        <DialogTrigger
          render={
            <Button variant="destructive" size="icon">
              <Trash />
            </Button>
          }
        />
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
