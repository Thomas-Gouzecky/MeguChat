import { useListen } from '@/components/signalr/useListen';
import {
  addTypingUser,
  removeTypingUser,
} from '@/store/slices/groupchatByIdSlice';
import { AppDispatch } from '@/store/store';
import { useDispatch } from 'react-redux';

type UserTypingEvent = {
  groupchatId: string;
  userId: string;
};

export function useGroupchatEvents() {
  const dispatch = useDispatch<AppDispatch>();

  useListen<[UserTypingEvent]>('UserTyping', (typingEvent) => {
    const { groupchatId, userId } = typingEvent;
    dispatch(
      addTypingUser({
        groupchat_id: groupchatId,
        user_id: userId,
      }),
    );
  });

  useListen<[UserTypingEvent]>('UserStoppedTyping', (typingEvent) => {
    const { groupchatId, userId } = typingEvent;
    dispatch(
      removeTypingUser({
        groupchat_id: groupchatId,
        user_id: userId,
      }),
    );
  });
}
