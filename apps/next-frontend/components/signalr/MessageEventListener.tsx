'use client';

import { useMessageEvents } from '@/hooks/useMessageEvents';

export function MessageEventListener() {
  useMessageEvents();
  return null; // This component does not render anything
}
