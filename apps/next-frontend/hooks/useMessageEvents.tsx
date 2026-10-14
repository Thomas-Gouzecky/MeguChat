import { useListen } from '@/components/signalr/useListen';
import { useDispatch } from 'react-redux';
import type { AppDispatch } from '@/store/store';
import {
  addMessage,
  editMessage,
  removeMessage,
} from '@/store/slices/messagesSlice';

export function useMessageEvents() {
  const dispatch = useDispatch<AppDispatch>();

  useListen<[MessageType]>('ReceiveMessage', (message) => {
    dispatch(
      addMessage({
        groupchat_id: message.group_chat_id.toString(),
        message,
      }),
    );
  });

  useListen<[MessageType]>('MessageUpdated', (message) => {
    dispatch(
      editMessage({
        groupchat_id: message.group_chat_id.toString(),
        message,
      }),
    );
  });

  useListen<[MessageType]>('MessageDeleted', (message) => {
    dispatch(
      removeMessage({
        groupchat_id: message.group_chat_id.toString(),
        message_id: message.id.toString(),
      }),
    );
  });
}
