import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchGroupchatMembers } from '../thunks/groupchatMembersThunk';

interface GroupchatMembersState {
  membersByGroupchatId: Record<string, GroupchatMember[]>;
  loadingByGroupchatId: Record<string, boolean>;
  errorByGroupchatId: Record<string, ErrorResponse | null>;
}

const initialState: GroupchatMembersState = {
  membersByGroupchatId: {},
  loadingByGroupchatId: {},
  errorByGroupchatId: {},
};

const groupchatMembersSlice = createSlice({
  name: 'groupchatMembers',
  initialState,
  reducers: {
    addMember: (
      state,
      action: PayloadAction<{ groupchat_id: string; member: GroupchatMember }>,
    ) => {
      state.membersByGroupchatId[action.payload.groupchat_id].push(
        action.payload.member,
      );
    },
    removeMember: (
      state,
      action: PayloadAction<{ groupchat_id: string; member_id: number }>,
    ) => {
      state.membersByGroupchatId[action.payload.groupchat_id] =
        state.membersByGroupchatId[action.payload.groupchat_id]?.filter(
          (member) => member.id !== action.payload.member_id,
        ) ?? [];
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGroupchatMembers.pending, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = true;
        state.errorByGroupchatId[action.meta.arg] = null;
      })
      .addCase(fetchGroupchatMembers.fulfilled, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = false;
        state.membersByGroupchatId[action.meta.arg] = action.payload;
      })
      .addCase(fetchGroupchatMembers.rejected, (state, action) => {
        state.loadingByGroupchatId[action.meta.arg] = false;
        state.errorByGroupchatId[action.meta.arg] = action.payload ?? null;
      });
  },
});

export const { addMember, removeMember } = groupchatMembersSlice.actions;
export default groupchatMembersSlice.reducer;
