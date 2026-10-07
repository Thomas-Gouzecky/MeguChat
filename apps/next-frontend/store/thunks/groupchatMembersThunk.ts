import { getGroupchatMembers } from '@/lib/api/groupchatMembers';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGroupchatMembers = createAsyncThunk<
  GroupchatMember[],
  number,
  { rejectValue: ErrorResponse }
>(
  'groupchats/fetchGroupchatMembers',
  async (groupchatId: number, { rejectWithValue }) => {
    const response = await getGroupchatMembers(groupchatId);

    if ('status' in response && response.status >= 400) {
      return rejectWithValue(response);
    }

    return response as GroupchatMember[];
  },
);
