import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';
import { getCurrentUser } from './lib/auth/auth';

export async function proxy(request: NextRequest) {
  const user = await getCurrentUser(request.headers.get('cookie') ?? '');

  if (!user) {
    return NextResponse.redirect(new URL('/login', request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ['/chat/:path*', '/profile/:path*', '/settings/:path*', '/'],
};
