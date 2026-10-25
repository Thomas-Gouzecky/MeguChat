from unittest.mock import AsyncMock

import pytest

from app.clients.members_client import MembersClient
from tests.helper import context_with_headers


@pytest.mark.anyio
async def test_get_members_of_groupchat_delegates_request():
    client = MembersClient()
    response = [
        {"user_id": "user-1", "username": "alice"},
        {"user_id": "user-2", "username": "bob"},
    ]
    client.make_request = AsyncMock(return_value=response)
    context = context_with_headers({"cookie": "session=abc"})

    result = await client.get_members_of_groupchat(context, 7)

    assert result == response
    client.make_request.assert_awaited_once_with(
        ctx=context,
        method="GET",
        endpoint="/groupchats/7/members",
        data={},
    )


@pytest.mark.anyio
async def test_add_member_to_groupchat_delegates_request():
    client = MembersClient()
    response = {"groupchat_id": 7, "added_user_ids": ["user-1", "user-2"]}
    client.make_request = AsyncMock(return_value=response)
    context = context_with_headers({"cookie": "session=abc"})

    result = await client.add_members_to_groupchat(
        context,
        7,
        ["user-1", "user-2"],
    )

    assert result == response
    client.make_request.assert_awaited_once_with(
        ctx=context,
        method="POST",
        endpoint="/groupchats/7/members",
        data={"userIds": ["user-1", "user-2"]},
    )


@pytest.mark.anyio
async def test_remove_member_from_groupchat_delegates_request():
    client = MembersClient()
    response = {"groupchat_id": 7, "removed_user_id": "user-1"}
    client.make_request = AsyncMock(return_value=response)
    context = context_with_headers({"cookie": "session=abc"})

    result = await client.remove_member_from_groupchat(context, 7, "user-1")

    assert result == response
    client.make_request.assert_awaited_once_with(
        ctx=context,
        method="DELETE",
        endpoint="/groupchats/7/members/user-1",
        data={},
    )
