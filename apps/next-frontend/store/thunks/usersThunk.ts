import { getAllUsers } from '@/lib/api/users';
import { createAsyncThunk } from '@reduxjs/toolkit';
import type { RootState } from '../store';

export const fetchUsers = createAsyncThunk<
  User[],
  void,
  { rejectValue: ErrorResponse; state: RootState }
>('users/fetchUsers', async (_, { rejectWithValue, getState }) => {
  const response = await getAllUsers();

  if ('status' in response && response.status >= 400) {
    return rejectWithValue(response);
  }

  const users = response as User[];
  const currentUserId = getState().auth.user?.user_id;

  const filteredUsers = currentUserId
    ? users.filter((user) => user.user_id !== currentUserId)
    : users;

  return filteredUsers;
});
