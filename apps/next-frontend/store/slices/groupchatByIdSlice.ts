import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchGroupchatById } from '../thunks/groupchatThunk';

interface GroupchatByIdState {
  groupchatsByGroupchatId: Record<string, Groupchat>;
  loadingByGroupchatId: Record<string, boolean>;
  errorByGroupchatId: Record<string, ErrorResponse | null>;
}
const initialStateById: GroupchatByIdState = {
  groupchatsByGroupchatId: {},
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

export const { addGroupchatById, updateGroupchatById, removeGroupchatById } =
  groupchatByIdSlice.actions;
export default groupchatByIdSlice.reducer;
