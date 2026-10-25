from unittest.mock import AsyncMock, Mock

import pytest
from app.tools.members_tool import add_members_to_groupchat
from tests.helper import context_with_headers


@pytest.mark.anyio
async def test_add_members_to_groupchat_forwards_request(monkeypatch):
    gateway_response = {
        "groupchat_id": 7,
        "added_user_ids": ["user-1", "user-2"],
    }
    response = Mock()
    response.json.return_value = gateway_response
    response.raise_for_status = Mock()

    http_client = AsyncMock()
    http_client.__aenter__.return_value = http_client
    http_client.request.return_value = response
    async_client = Mock(return_value=http_client)
    monkeypatch.setattr(
        "app.clients.gateway_client.httpx.AsyncClient",
        async_client,
    )

    result = await add_members_to_groupchat(
        context_with_headers({"cookie": "session=abc"}),
        7,
        ["user-1", "user-2"],
    )

    assert result == gateway_response
    http_client.request.assert_awaited_once_with(
        "POST",
        "http://localhost:5192/api/groupchats/7/members",
        json={"userIds": ["user-1", "user-2"]},
        headers={"Cookie": "session=abc"},
    )
    response.raise_for_status.assert_called_once_with()


@pytest.mark.anyio
async def test_add_members_to_groupchat_rejects_missing_cookie():
    with pytest.raises(ValueError, match="Missing cookie in request headers."):
        await add_members_to_groupchat(context_with_headers({}), 7, ["user-1"])