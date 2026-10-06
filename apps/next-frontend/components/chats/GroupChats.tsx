'use client';

import { useEffect, useState } from 'react';

import { EmptyGroupchats } from '@/components/chats/EmptyGroupchats';
import { getUsersGroupchats } from '@/lib/api/groupchats';

export default function Groupchats() {
  const [groupchats, setGroupchats] = useState<Groupchat[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let isMounted = true;

    async function fetchGroupchats() {
      const response = await getUsersGroupchats();

      if (!isMounted) {
        return;
      }

      if (Array.isArray(response)) {
        setGroupchats(response);
      } else {
        console.error('Error fetching group chats:', response.detail);
      }

      setIsLoading(false);
    }

    fetchGroupchats();

    return () => {
      isMounted = false;
    };
  }, []);

  // Loading state
  if (isLoading) {
    return (
      <div className="p-4 text-sm text-muted-foreground">
        Loading group chats...
      </div>
    );
  }

  // User has no group chats
  if (groupchats.length === 0) {
    return <EmptyGroupchats />;
  }

  // Render the actual group chats
  return (
    <div className="space-y-3 p-4">
      {groupchats.map((groupchat) => (
        <div
          key={groupchat.id}
          className="rounded-lg border border-border bg-card p-4 shadow-sm"
        >
          <p className="text-lg font-semibold">{groupchat.name}</p>
          <p className="text-sm text-muted-foreground">
            Group chat ID: {groupchat.id}
          </p>
        </div>
      ))}
    </div>
  );
}
