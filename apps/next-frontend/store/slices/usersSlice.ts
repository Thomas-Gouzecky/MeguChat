import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchUsers } from '../thunks/usersThunk';

interface UsersState {
  users: User[];
  isLoading: boolean;
  error: ErrorResponse | null;
}

const initialState: UsersState = {
  users: [],
  isLoading: false,
  error: null,
};
const usersSlice = createSlice({
  name: 'users',
  initialState,
  reducers: {
    setUsers: (state, action: PayloadAction<User[]>) => {
      state.users = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchUsers.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchUsers.fulfilled, (state, action) => {
        state.isLoading = false;
        state.users = action.payload;
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload ?? null;
      });
  },
});

export const { setUsers } = usersSlice.actions;
export default usersSlice.reducer;
