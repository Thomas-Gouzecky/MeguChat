import { updateMessage } from '@/lib/api/messages';
import { editMessage } from '@/store/slices/messagesSlice';
import { AppDispatch } from '@/store/store';
import { Button } from '@meguchat/ui/components/ui/button';
import { Textarea } from '@meguchat/ui/components/ui/textarea';
import { Check, X } from 'lucide-react';
import { useState } from 'react';
import { useDispatch } from 'react-redux';

type EditMessageBoxProps = {
  message: MessageType;
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

export function EditableMessageBox({
  message,
  open,
  onOpenChange,
}: EditMessageBoxProps) {
  const dispatch = useDispatch<AppDispatch>();
  const [content, setContent] = useState(message.content);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState(false);

  if (!open) {
    return null;
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmedContent = content.trim();

    if (!trimmedContent) {
      return;
    }

    setIsSaving(true);
    setError(false);

    try {
      const response = await updateMessage(
        message.group_chat_id.toString(),
        message.id.toString(),
        trimmedContent,
      );

      if (!('id' in response)) {
        setError(true);
        return;
      }

      dispatch(
        editMessage({
          groupchat_id: message.group_chat_id.toString(),
          message: { ...response, status: message.status ?? 'sent' },
        }),
      );
      onOpenChange(false);
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="flex min-w-0 max-w-full items-end gap-2"
    >
      <Textarea
        className="min-h-10 max-h-48 w-auto min-w-12 max-w-full resize-none overflow-y-auto"
        value={content}
        onChange={(event) => setContent(event.target.value)}
        autoFocus
        disabled={isSaving}
        aria-label="Edit message"
        rows={1}
      />
      <Button
        type="submit"
        size="icon-sm"
        disabled={isSaving || !content.trim()}
        aria-label="Save message"
        title="Save message"
      >
        <Check />
      </Button>
      <Button
        type="button"
        variant="outline"
        size="icon-sm"
        disabled={isSaving}
        aria-label="Cancel editing"
        title="Cancel editing"
        onClick={() => onOpenChange(false)}
      >
        <X />
      </Button>
      {error && (
        <span className="text-sm text-destructive" role="alert">
          Unable to save
        </span>
      )}
    </form>
  );
}
