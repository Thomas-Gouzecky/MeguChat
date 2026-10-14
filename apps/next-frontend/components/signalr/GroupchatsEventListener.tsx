import { useGroupchatEvents } from '@/hooks/useGroupchatEvents';

export default function GroupchatsEventListener() {
  useGroupchatEvents();
  return null;
}
