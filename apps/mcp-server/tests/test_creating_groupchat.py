from types import SimpleNamespace
from typing import cast
from unittest.mock import AsyncMock, Mock

import app.tools
import pytest
from app.tools.groupchat import create_groupchat
from mcp.server.mcpserver.context import Context


@pytest.mark.anyio
async def test_create_groupchat_forwards_context_cookie(monkeypatch):
    gateway_response = {
        "groupchat_id": 1,
        "name": "test-name",
        "created_at": "2026-10-24T20:31:24Z",
    }
    response = Mock()
    response.json.return_value = gateway_response
    response.raise_for_status = Mock()

    http_client = AsyncMock()
    http_client.__aenter__.return_value = http_client
    http_client.request.return_value = response
    async_client = Mock(return_value=http_client)
    monkeypatch.setattr("app.clients.gateway_client.httpx.AsyncClient", async_client)

    context = cast(
        Context,
        SimpleNamespace(headers={"cookie": "test-cookie"}),
    )
    result = await create_groupchat(
        context,
        "test-name",
        ["user1", "user2"],
    )

    assert result == gateway_response
    async_client.assert_called_once_with()
    http_client.request.assert_awaited_once_with(
        "POST",
        "http://localhost:5192/api/groupchats",
        json={"name": "test-name", "users": ["user1", "user2"]},
        headers={"Cookie": "test-cookie"},
    )


@pytest.mark.anyio
async def test_create_groupchat_rejects_missing_cookie():
    context = cast(Context, SimpleNamespace(headers={}))
    with pytest.raises(ValueError, match="Missing cookie in request headers."):
        await create_groupchat(
            context,
            "test-name",
            ["user1", "user2"],
        )
