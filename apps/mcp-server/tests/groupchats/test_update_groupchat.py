from unittest.mock import AsyncMock, Mock

import pytest
from app.DTO import UpdateGroupchatDTO
from app.clients.groupchat_client import GroupchatClient
from app.tools import update_groupchat

from tests.helper import context_with_headers


@pytest.mark.anyio
async def test_groupchat_client_delegates_update_request():
    client = GroupchatClient()
    response = {"groupchat_id": 7, "name": "updated friends"}
    client.make_request = AsyncMock(return_value=response)
    context = context_with_headers({"cookie": "session=abc"})

    result = await client.update_groupchat(
        context,
        7,
        update_data=UpdateGroupchatDTO(name="updated friends"),
    )

    assert result == response
    client.make_request.assert_awaited_once_with(
        ctx=context,
        method="PUT",
        endpoint="/groupchats/7",
        data={"name": "updated friends"},
    )


@pytest.mark.anyio
async def test_update_groupchat_forwards_context_cookie(monkeypatch):
    gateway_response = {"groupchat_id": 7, "name": "updated friends"}
    response = Mock()
    response.json.return_value = gateway_response
    response.raise_for_status = Mock()

    http_client = AsyncMock()
    http_client.__aenter__.return_value = http_client
    http_client.request.return_value = response
    async_client = Mock(return_value=http_client)
    monkeypatch.setattr("app.clients.gateway_client.httpx.AsyncClient", async_client)

    context = context_with_headers({"cookie": "session=abc"})
    result = await update_groupchat(context, 7, "updated friends")

    assert result == gateway_response
    http_client.request.assert_awaited_once_with(
        "PUT",
        "http://localhost:5192/api/groupchats/7",
        json={"name": "updated friends"},
        headers={"Cookie": "session=abc"},
    )
    response.raise_for_status.assert_called_once_with()


@pytest.mark.anyio
async def test_update_groupchat_rejects_missing_cookie():
    context = context_with_headers({})

    with pytest.raises(ValueError, match="Missing cookie in request headers."):
        await update_groupchat(context, 7, "updated friends")
