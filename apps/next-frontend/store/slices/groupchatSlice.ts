import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchGroupchats } from '../thunks/groupchatThunk';

interface GroupchatState {
  groupchats: Groupchat[];
  loading: boolean;
  error: ErrorResponse | null;
}

const initialState: GroupchatState = {
  groupchats: [],
  loading: false,
  error: null,
};

const groupchatSlice = createSlice({
  name: 'groupchats',
  initialState,
  reducers: {
    addGroupchat: (state, action: PayloadAction<Groupchat>) => {
      state.groupchats.push(action.payload);
    },
    updateGroupchat: (state, action: PayloadAction<Groupchat>) => {
      const index = state.groupchats.findIndex(
        (groupchat) => groupchat.groupchat_id === action.payload.groupchat_id,
      );

      if (index !== -1) {
        state.groupchats[index] = action.payload;
      }
    },
    removeGroupchat: (state, action: PayloadAction<number>) => {
      state.groupchats = state.groupchats.filter(
        (groupchat) => groupchat.groupchat_id !== action.payload,
      );
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGroupchats.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchGroupchats.fulfilled, (state, action) => {
        state.loading = false;
        state.groupchats = action.payload;
      })
      .addCase(fetchGroupchats.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload ?? null;
      });
  },
});

export const { addGroupchat, updateGroupchat, removeGroupchat } =
  groupchatSlice.actions;
export default groupchatSlice.reducer;
