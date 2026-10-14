import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchGroupchatById } from '../thunks/groupchatThunk';

interface GroupchatByIdState {
  groupchatsByGroupchatId: Record<string, Groupchat>;
  typingUsersByGroupchatId: Record<string, string[]>;
  loadingByGroupchatId: Record<string, boolean>;
  errorByGroupchatId: Record<string, ErrorResponse | null>;
}
const initialStateById: GroupchatByIdState = {
  groupchatsByGroupchatId: {},
  typingUsersByGroupchatId: {},
  loadingByGroupchatId: {},
  errorByGroupchatId: {},
};

const groupchatByIdSlice = createSlice({
  name: 'groupchatById',
  initialState: initialStateById,
  reducers: {
    addGroupchatById: (
      state,
      action: PayloadAction<{ groupchat_id: string; groupchat: Groupchat }>,
    ) => {
      state.groupchatsByGroupchatId[action.payload.groupchat_id] =
        action.payload.groupchat;
    },
    updateGroupchatById: (
      state,
      action: PayloadAction<{ groupchat_id: string; groupchat: Groupchat }>,
    ) => {
      state.groupchatsByGroupchatId[action.payload.groupchat_id] =
        action.payload.groupchat;
    },
    removeGroupchatById: (state, action: PayloadAction<string>) => {
      delete state.groupchatsByGroupchatId[action.payload];
    },
    addTypingUser: (
      state,
      action: PayloadAction<{ groupchat_id: string; user_id: string }>,
    ) => {
      const { groupchat_id, user_id } = action.payload;
      state.typingUsersByGroupchatId[groupchat_id] ??= [];
      if (!state.typingUsersByGroupchatId[groupchat_id].includes(user_id)) {
        state.typingUsersByGroupchatId[groupchat_id].push(user_id);
      }
    },
    removeTypingUser: (
      state,
      action: PayloadAction<{ groupchat_id: string; user_id: string }>,
    ) => {
      const { groupchat_id, user_id } = action.payload;
      state.typingUsersByGroupchatId[groupchat_id] ??= [];
      state.typingUsersByGroupchatId[groupchat_id] =
        state.typingUsersByGroupchatId[groupchat_id].filter(
          (id) => id !== user_id,
        );
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGroupchatById.pending, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = true;
        state.errorByGroupchatId[action.meta.arg] = null;
      })
      .addCase(fetchGroupchatById.fulfilled, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = false;
        state.groupchatsByGroupchatId[action.meta.arg] = action.payload;
      })
      .addCase(fetchGroupchatById.rejected, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = false;
        state.errorByGroupchatId[action.meta.arg] = action.payload ?? null;
      });
  },
});

export const {
  addGroupchatById,
  updateGroupchatById,
  removeGroupchatById,
  addTypingUser,
  removeTypingUser,
} = groupchatByIdSlice.actions;
export default groupchatByIdSlice.reducer;
