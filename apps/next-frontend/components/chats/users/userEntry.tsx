import { Avatar, AvatarFallback } from '@meguchat/ui/components/ui/avatar';
import {
  Item,
  ItemActions,
  ItemContent,
  ItemMedia,
  ItemTitle,
} from '@meguchat/ui/components/ui/item';
import { useState } from 'react';

export function UserEntry({
  className,
  user,
}: {
  className?: string;
  user: User;
}) {
  const [selected, setSelected] = useState(false);
  return (
    <Item
      variant="outline"
      onClick={() => setSelected(!selected)}
      className={className}
    >
      <ItemMedia>
        <Avatar>
          <AvatarFallback>
            {user.username.charAt(0).toUpperCase()}
          </AvatarFallback>
        </Avatar>
      </ItemMedia>
      <ItemContent>
        <ItemTitle>{user.username}</ItemTitle>
      </ItemContent>
      <ItemActions>
        <input
          type="checkbox"
          checked={selected}
          name="users"
          value={user.user_id}
          onChange={() => setSelected(!selected)}
        />
      </ItemActions>
    </Item>
  );
}
