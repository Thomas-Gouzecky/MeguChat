from unittest.mock import AsyncMock, Mock

import pytest
from app.tools.members_tool import remove_member_from_groupchat
from tests.helper import context_with_headers


@pytest.mark.anyio
async def test_remove_member_from_groupchat_forwards_request(monkeypatch):
    gateway_response = {
        "groupchat_id": 7,
        "removed_user_id": "user-1",
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

    result = await remove_member_from_groupchat(
        context_with_headers({"cookie": "session=abc"}),
        7,
        "user-1",
    )

    assert result == gateway_response
    http_client.request.assert_awaited_once_with(
        "DELETE",
        "http://localhost:5192/api/groupchats/7/members/user-1",
        json={},
        headers={"Cookie": "session=abc"},
    )
    response.raise_for_status.assert_called_once_with()


@pytest.mark.anyio
async def test_remove_member_from_groupchat_rejects_missing_cookie():
    with pytest.raises(ValueError, match="Missing cookie in request headers."):
        await remove_member_from_groupchat(
            context_with_headers({}),
            7,
            "user-1",
        )