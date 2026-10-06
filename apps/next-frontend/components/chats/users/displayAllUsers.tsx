import { useDispatch, useSelector } from 'react-redux';
import { UserEntry } from './userEntry';
import { AppDispatch, RootState } from '@/store/store';
import { useEffect, useState } from 'react';
import { fetchUsers } from '@/store/thunks/usersThunk';

export function DisplayAllUsers() {
  const [selectedUserIds, setSelectedUserIds] = useState<number[]>([]);
  const dispatch = useDispatch<AppDispatch>();
  const { users, isLoading, error } = useSelector(
    (state: RootState) => state.users,
  );

  useEffect(() => {
    void dispatch(fetchUsers());
  }, [dispatch]);

  if (isLoading) {
    return <div>Loading users...</div>;
  }

  if (error) {
    return (
      <div className="text-destructive">
        {error.title}: {error.detail}
      </div>
    );
  }

  return (
    <div>
      {users?.map((user) => (
        <UserEntry
          key={user.user_id}
          user={user}
          isSelected={selectedUserIds.includes(user.user_id)}
          onSelect={(userId) =>
            setSelectedUserIds((currentIds) =>
              currentIds.includes(userId)
                ? currentIds.filter((id) => id !== userId)
                : [...currentIds, userId],
            )
          }
        />
      ))}
    </div>
  );
}
