'use client';

import { useSelector } from 'react-redux';
import type { RootState } from '@/store/store';
import { SignalRProvider } from './SignalRContext';
import { MessageEventListener } from './MessageEventListener';

export function AuthenticatedSignalRProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const user = useSelector((state: RootState) => state.auth.user);

  if (!user) {
    return <>{children}</>;
  }

  const serverUrl = process.env.NEXT_PUBLIC_SIGNALR_URL;

  if (!serverUrl) {
    console.error('SignalR URL is not configured.');
    return <>{children}</>;
  }

  return (
    <SignalRProvider serverUrl={serverUrl}>
      <MessageEventListener />
      {children}
    </SignalRProvider>
  );
}
