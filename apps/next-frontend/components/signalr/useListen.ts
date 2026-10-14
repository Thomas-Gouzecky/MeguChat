'use client';

import { useEffect, useRef } from 'react';
import { useSignalR } from './SignalRContext';

export function useListen<TArgs extends unknown[]>(
  eventName: string,
  callback: (...args: TArgs) => void,
) {
  const { connection } = useSignalR();
  const callbackRef = useRef(callback);

  useEffect(() => {
    callbackRef.current = callback;
  }, [callback]);

  useEffect(() => {
    if (!connection || !eventName) return;

    const handler = (...args: TArgs) => {
      callbackRef.current(...args);
    };

    connection.on(eventName, handler);

    return () => {
      connection.off(eventName, handler);
    };
  }, [connection, eventName]);
}
