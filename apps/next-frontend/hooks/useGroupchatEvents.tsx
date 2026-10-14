import { useListen } from '@/components/signalr/useListen';
import { AppDispatch } from '@/store/store';
import { useDispatch } from 'react-redux';

export function useGroupchatEvents() {
  const dispatch = useDispatch<AppDispatch>();

  useListen<[string]>('UserTyping', (userId) => {
    console.log(`User ${userId} is typing...`);
  });

  useListen<[string]>('UserStoppedTyping', (userId) => {
    console.log(`User ${userId} stopped typing.`);
  });
}
