import authReducer from './slices/authSlice';
import usersReducer from './slices/usersSlice';
import groupchatReducer from './slices/groupchatSlice';
import groupchatMembersReducer from './slices/groupchatMembersSlice';
import messagesReducer from './slices/messagesSlice';
import { configureStore } from '@reduxjs/toolkit';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    users: usersReducer,
    groupchats: groupchatReducer,
    groupchatMembers: groupchatMembersReducer,
    messages: messagesReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
