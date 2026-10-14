import { useListen } from '@/components/signalr/useListen';
import {
  addTypingUser,
  removeTypingUser,
} from '@/store/slices/groupchatByIdSlice';
import { AppDispatch, RootState } from '@/store/store';
import { useDispatch, useSelector } from 'react-redux';

type UserTypingEvent = {
  groupchat_id: string;
  user_id: string;
};

export function useGroupchatEvents() {
  const dispatch = useDispatch<AppDispatch>();
  const userId = useSelector((state: RootState) => state.auth.user?.user_id);
  useListen<[UserTypingEvent]>('UserTyping', (typingEvent) => {
    const { groupchat_id, user_id } = typingEvent;
    if (userId === user_id) {
      return;
    }

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
