import { useDispatch, useSelector } from 'react-redux';
import { UserEntry } from './userEntry';
import { AppDispatch, RootState } from '@/store/store';
import { useEffect } from 'react';
import { fetchUsers } from '@/store/thunks/usersThunk';

export function DisplayAllUsers() {
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
    <>
      {users?.map((user) => (
        <UserEntry className="mb-4" key={user.user_id} user={user} />
      ))}
    </>
  );
}
