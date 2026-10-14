'use client';

import { MessagingInterface } from '@/components/message/Groupchat-Messages';
import { useSignalR } from '@/components/signalr/SignalRContext';
import { HubConnectionState } from '@microsoft/signalr';
import { useEffect } from 'react';
import { use } from 'react';

export default function GroupchatViewPage({
  params,
}: {
  params: Promise<{ groupchat_id: string }>;
}) {
  const { groupchat_id } = use(params);
  const { connection, status } = useSignalR();

  useEffect(() => {
    if (status !== 'Connected' || !groupchat_id) return;
    if (!connection) {
      console.error('SignalR connection is not established.');
      return;
    }

    let cancelled = false;

    async function joinGroupchat() {
      try {
        await connection?.invoke('JoinGroupchat', Number(groupchat_id));
      } catch (error) {
        if (!cancelled) {
          console.error('Failed to join group chat:', error);
        }
      }
    }

    joinGroupchat();

    return () => {
      cancelled = true;

      if (connection && connection.state === HubConnectionState.Connected) {
        void connection
          .invoke('LeaveGroupchat', Number(groupchat_id))
          .catch((error) => {
            console.error('Failed to leave group chat:', error);
          });
      }
    };
  }, [connection, status, groupchat_id]);

  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background">
      <MessagingInterface groupchatId={groupchat_id} />
    </div>
  );
}
