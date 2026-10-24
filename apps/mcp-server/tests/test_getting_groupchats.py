from types import SimpleNamespace
from typing import cast
from unittest.mock import AsyncMock, Mock

import app.tools
import pytest
from app.tools.groupchat import get_groupchat
from mcp.server.mcpserver.context import Context
from app.clients.groupchat_client import GroupchatClient


def context_with_headers(headers: dict[str, str] | None) -> Context:
    return cast(Context, SimpleNamespace(headers=headers))


@pytest.mark.anyio
async def test_groupchat_client_delegates_get_request():
    client = GroupchatClient()
    response = {"groupchat_id": 7, "name": "friends", "users": ["user-1"]}
    client.make_request = AsyncMock(return_value=response)
    context = context_with_headers({"cookie": "session=abc"})

    result = await client.get_groupchat(context, 7)

    assert result == response
    client.make_request.assert_awaited_once_with(
        ctx=context,
        method="GET",
        endpoint="/groupchats/7",
        data={},
    )


@pytest.mark.anyio
async def test_get_groupchat_forwards_context_cookie(monkeypatch):
    gateway_response = {
        "groupchat_id": 7,
        "name": "friends",
        "users": ["user-1", "user-2"],
    }
    response = Mock()
    response.json.return_value = gateway_response
    response.raise_for_status = Mock()

    http_client = AsyncMock()
    http_client.__aenter__.return_value = http_client
    http_client.request.return_value = response
    async_client = Mock(return_value=http_client)
    monkeypatch.setattr("app.clients.gateway_client.httpx.AsyncClient", async_client)

    context = context_with_headers({"cookie": "session=abc"})
    result = await get_groupchat(context, 7)

    assert result == gateway_response
    http_client.request.assert_awaited_once_with(
        "GET",
        "http://localhost:5192/api/groupchats/7",
        json={},
        headers={"Cookie": "session=abc"},
    )
    response.raise_for_status.assert_called_once_with()


@pytest.mark.anyio
async def test_get_groupchat_rejects_missing_cookie():
    context = context_with_headers({})

    with pytest.raises(ValueError, match="Missing cookie in request headers."):
        await get_groupchat(context, 7)
