'use client';

import { configureStore } from '@reduxjs/toolkit';
import { Provider } from 'react-redux';
import authReducer from './authSlice';

export default function StoreProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const store = configureStore({
    reducer: {
      auth: authReducer,
    },
  });
  return <Provider store={store}>{children}</Provider>;
}
