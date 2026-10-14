import { useListen } from '@/components/signalr/useListen';
import {
  addTypingUser,
  removeTypingUser,
} from '@/store/slices/groupchatByIdSlice';
import { AppDispatch } from '@/store/store';
import { useDispatch } from 'react-redux';

type UserTypingEvent = {
  groupchat_id: string;
  user_id: string;
};

export function useGroupchatEvents() {
  const dispatch = useDispatch<AppDispatch>();

  useListen<[UserTypingEvent]>('UserTyping', (typingEvent) => {
    const { groupchat_id, user_id } = typingEvent;
    dispatch(
      addTypingUser({
        groupchat_id: groupchat_id,
        user_id: user_id,
      }),
    );
  });

  useListen<[UserTypingEvent]>('UserStoppedTyping', (typingEvent) => {
    const { groupchat_id, user_id } = typingEvent;
    dispatch(
      removeTypingUser({
        groupchat_id: groupchat_id,
        user_id: user_id,
      }),
    );
  });
}
