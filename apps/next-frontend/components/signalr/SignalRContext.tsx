'use client';

import { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

type SignalRContextValue = {
  connection: signalR.HubConnection | null;
  status: signalR.HubConnectionState | 'Disconnected';
};

// Context to provide SignalR connection and status to components
export const SignalRContext = createContext<SignalRContextValue>({
  connection: null,
  status: 'Disconnected',
});

type SignalRProviderProps = {
  children: React.ReactNode;
  serverUrl: string;
};

// SignalRProvider component to manage SignalR connection and provide context
export function SignalRProvider({ children, serverUrl }: SignalRProviderProps) {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );
  const [status, setStatus] = useState<
    signalR.HubConnectionState | 'Disconnected'
  >('Disconnected');

  useEffect(() => {
    if (!serverUrl) return;

    let active = true;

    const hub = new signalR.HubConnectionBuilder()
      .withUrl(serverUrl)
      .withAutomaticReconnect([0, 2000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Information)
      .build();

    const updateStatus = () => {
      if (active) setStatus(hub.state);
    };

    hub.onreconnecting(updateStatus);
    hub.onreconnected(updateStatus);
    hub.onclose(updateStatus);

    // eslint-disable-next-line react-hooks/set-state-in-effect
    setConnection(hub);

    async function start() {
      try {
        await hub.start();

        if (active) setStatus(hub.state);
      } catch (error) {
        console.error('Error starting SignalR connection:', error);
      }
    }

    void start();

    return () => {
      active = false;
      void hub
        .stop()
        .catch((error) =>
          console.error('Error stopping SignalR connection:', error),
        );
      setConnection(null);
      setStatus('Disconnected');
    };
  }, [serverUrl]);

  return (
    <SignalRContext.Provider value={{ connection, status }}>
      {children}
    </SignalRContext.Provider>
  );
}

// Custom hook to access SignalR context
export function useSignalR() {
  const context = useContext(SignalRContext);

  if (!context) {
    throw new Error('useSignalR must be used within a SignalRProvider');
  }

  return context;
}
