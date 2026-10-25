from unittest.mock import AsyncMock, Mock

import pytest
from app.tools.members_tool import get_members_of_groupchat
from tests.helper import context_with_headers


@pytest.mark.anyio
async def test_get_members_of_groupchat_forwards_request(monkeypatch):
    gateway_response = [
        {"user_id": "user-1", "username": "alice"},
        {"user_id": "user-2", "username": "bob"},
    ]
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

    result = await get_members_of_groupchat(
        context_with_headers({"cookie": "session=abc"}),
        7,
    )

    assert result == gateway_response
    http_client.request.assert_awaited_once_with(
        "GET",
        "http://localhost:5192/api/groupchats/7/members",
        json={},
        headers={"Cookie": "session=abc"},
    )
    response.raise_for_status.assert_called_once_with()


@pytest.mark.anyio
async def test_get_members_of_groupchat_rejects_missing_cookie():
    with pytest.raises(ValueError, match="Missing cookie in request headers."):
        await get_members_of_groupchat(context_with_headers({}), 7)