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

export const fetchGroupchatById = createAsyncThunk<
  Groupchat,
  string,
  { rejectValue: ErrorResponse }
>(
  'groupchats/fetchGroupchatById',
  async (groupchatId: string, { rejectWithValue }) => {
    const response = await getGroupchatById(groupchatId);

    if ('status' in response && response.status >= 400) {
      return rejectWithValue(response);
    }

    return response as Groupchat;
  },
);
