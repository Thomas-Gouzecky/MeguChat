import json
from unittest.mock import AsyncMock, Mock

import app.tools


async def test_call_create_groupchat_endpoint(client, monkeypatch):
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
    http_client.post.return_value = response
    async_client = Mock(return_value=http_client)
    monkeypatch.setattr("app.tools.groupchat.httpx.AsyncClient", async_client)

    result = await client.call_tool(
        "create-groupchat", {"name": "test-name", "users": ["user1", "user2"]}
    )

    assert json.loads(result.content[0].text) == gateway_response
    async_client.assert_called_once_with()
    http_client.post.assert_awaited_once_with(
        "http://localhost:5192/api/groupchats",
        json={"name": "test-name", "users": ["user1", "user2"]},
    )
