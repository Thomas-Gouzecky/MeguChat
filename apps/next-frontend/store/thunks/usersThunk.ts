import { getAllUsers } from '@/lib/api/users';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchUsers = createAsyncThunk<
  User[],
  void,
  { rejectValue: ErrorResponse }
>('users/fetchUsers', async (_, { rejectWithValue }) => {
  const response = await getAllUsers();

  if ('status' in response && response.status >= 400) {
    return rejectWithValue(response);
  }

  return response as User[];
});
