import { getMessagesByGroupchatId } from '@/lib/api/messages';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchMessagesByGroupchatId = createAsyncThunk<
  MessageType[],
  number,
  { rejectValue: ErrorResponse }
>(
  'groupchats/{groupchatId}/messages',
  async (groupchatId: number, { rejectWithValue }) => {
    const response = await getMessagesByGroupchatId(groupchatId);

    if ('status' in response && response.status >= 400) {
      return rejectWithValue(response);
    }

    return response as MessageType[];
  },
);
