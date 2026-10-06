'use client';

import { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { setUser, clearUser } from '@/store/slices/authSlice';

interface AuthInitializerProps {
  user: {
    username: string;
    user_id: string;
  } | null;
}

export default function AuthInitializer({ user }: AuthInitializerProps) {
  const dispatch = useDispatch();

  useEffect(() => {
    if (user) {
      dispatch(setUser(user));
    } else {
      dispatch(clearUser());
    }
  }, [user, dispatch]);

  return null;
}
