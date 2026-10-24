from types import SimpleNamespace
from typing import cast
from unittest.mock import AsyncMock, Mock

import pytest
from mcp.server.mcpserver.context import Context

from app.clients.base.client import BaseClient
from app.clients.gateway_client import GatewayClient
from app.clients.groupchat_client import GroupchatClient


def context_with_headers(headers: dict[str, str] | None) -> Context:
    return cast(Context, SimpleNamespace(headers=headers))


def test_base_client_returns_cookie_from_request_headers():
    context = context_with_headers({"cookie": "session=abc"})

    assert BaseClient().get_cookie(context) == "session=abc"


@pytest.mark.parametrize("headers", [None, {}, {"cookie": ""}])
def test_base_client_rejects_missing_cookie(headers):
    with pytest.raises(ValueError, match="Missing cookie in request headers."):
        BaseClient().get_cookie(context_with_headers(headers))


@pytest.mark.anyio
async def test_gateway_client_makes_request_with_gateway_url_and_cookie(monkeypatch):
    gateway_response = {"status": "ok"}
    response = Mock()
    response.json.return_value = gateway_response
    http_client = AsyncMock()
    http_client.__aenter__.return_value = http_client
    http_client.request.return_value = response
    async_client = Mock(return_value=http_client)
    monkeypatch.setattr("app.clients.gateway_client.httpx.AsyncClient", async_client)

    result = await GatewayClient().make_request(
        context_with_headers({"cookie": "session=abc"}),
        "PATCH",
        "/resource",
        {"enabled": True},
    )

    assert result == gateway_response
    async_client.assert_called_once_with()
    http_client.request.assert_awaited_once_with(
        "PATCH",
        "http://localhost:5192/api/resource",
        json={"enabled": True},
        headers={"Cookie": "session=abc"},
    )
    response.raise_for_status.assert_called_once_with()


@pytest.mark.anyio
async def test_groupchat_client_delegates_create_request():
    client = GroupchatClient()
    client.make_request = AsyncMock(return_value={"groupchat_id": 7})
    context = context_with_headers({"cookie": "session=abc"})

    result = await client.create_groupchat(context, "friends", ["user-1", "user-2"])

    assert result == {"groupchat_id": 7}
    client.make_request.assert_awaited_once_with(
        ctx=context,
        method="POST",
        endpoint="/groupchats",
        data={"name": "friends", "users": ["user-1", "user-2"]},
    )
