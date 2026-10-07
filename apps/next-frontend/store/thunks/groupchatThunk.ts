import { getUsersGroupchats } from '@/lib/api/groupchats';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGroupchats = createAsyncThunk<
  Groupchat[],
  void,
  { rejectValue: ErrorResponse }
>('groupchats/fetchGroupchats', async (_, { rejectWithValue }) => {
  const response = await getUsersGroupchats();

  if ('status' in response && response.status >= 400) {
    return rejectWithValue(response);
  }

  return response as Groupchat[];
});
