import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import {
  createGroupchat,
  deleteGroupchat,
  editGroupchat,
  getUsersGroupchats,
} from '@/lib/api/groupchats';
import { getGroupchatMembers } from '@/lib/api/groupchatMembers';

describe('Group chat API', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn());
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
  });

  it('gets and maps group chats', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(
        JSON.stringify([
          { groupchat_id: 1, name: 'Team chat', created_at: '2026-10-06' },
        ]),
        { status: 200 },
      ),
    );

    await expect(getUsersGroupchats()).resolves.toEqual([
      { groupchat_id: 1, name: 'Team chat', created_at: '2026-10-06' },
    ]);
    expect(fetch).toHaveBeenCalledWith('/api/groupchats', {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
    });
  });

  it('creates a group chat with selected users', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(
        JSON.stringify({ groupchat_id: 2, name: 'New chat', created_at: null }),
        { status: 201 },
      ),
    );

    await expect(
      createGroupchat({ name: 'New chat', users: ['user-1', 'user-2'] }),
    ).resolves.toEqual({
      groupchat_id: 2,
      name: 'New chat',
      created_at: null,
    });
    expect(fetch).toHaveBeenCalledWith('/api/groupchats', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ name: 'New chat', users: ['user-1', 'user-2'] }),
    });
  });

  it('updates a group chat', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify({ groupchat_id: 3, name: 'Renamed chat' }), {
        status: 200,
      }),
    );

    await expect(
      editGroupchat(3, { name: 'Renamed chat', users: ['user-3'] }),
    ).resolves.toEqual({
      groupchat_id: 3,
      name: 'Renamed chat',
      created_at: undefined,
    });
    expect(fetch).toHaveBeenCalledWith('/api/groupchats/3', {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ name: 'Renamed chat', users: ['user-3'] }),
    });
  });

  it('deletes a group chat', async () => {
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify({ groupchat_id: 4, name: 'Deleted chat' }), {
        status: 200,
      }),
    );

    await expect(deleteGroupchat(4)).resolves.toEqual({
      groupchat_id: 4,
      name: 'Deleted chat',
      created_at: undefined,
    });
    expect(fetch).toHaveBeenCalledWith('/api/groupchats/4', {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
    });
  });

  it('gets members for a group chat', async () => {
    const members = [
      {
        id: 1,
        group_chat_id: 5,
        user_id: 'user-1',
        joined_at: '2026-10-06',
        last_active_at: '2026-10-06',
        last_read_message_id: null,
      },
    ];
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify(members), { status: 200 }),
    );

    await expect(getGroupchatMembers(5)).resolves.toEqual(members);
    expect(fetch).toHaveBeenCalledWith('/api/groupchats/5/members', {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
    });
  });

  it('returns API errors from group chat requests', async () => {
    const error = {
      title: 'Not found',
      status: 404,
      detail: 'Group chat not found',
    };
    vi.mocked(fetch).mockResolvedValue(
      new Response(JSON.stringify(error), { status: 404 }),
    );

    await expect(getUsersGroupchats()).resolves.toEqual(error);
  });
});
