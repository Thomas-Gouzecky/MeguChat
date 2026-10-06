import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchGroupchatMembers } from '../thunks/groupchatMembersThunk';

interface GroupchatMembersState {
  groupchatMembers: GroupchatMember[];
  loading: boolean;
  error: ErrorResponse | null;
}

const initialState: GroupchatMembersState = {
  groupchatMembers: [],
  loading: false,
  error: null,
};

const groupchatMembersSlice = createSlice({
  name: 'groupchatMembers',
  initialState,
  reducers: {
    addMember: (state, action: PayloadAction<GroupchatMember>) => {
      state.groupchatMembers.push(action.payload);
    },
    removeMember: (state, action: PayloadAction<number>) => {
      state.groupchatMembers = state.groupchatMembers.filter(
        (member) => member.id !== action.payload,
      );
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGroupchatMembers.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchGroupchatMembers.fulfilled, (state, action) => {
        state.loading = false;
        state.groupchatMembers = action.payload;
      })
      .addCase(fetchGroupchatMembers.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload ?? null;
      });
  },
});

export const { addMember, removeMember } = groupchatMembersSlice.actions;
export default groupchatMembersSlice.reducer;
