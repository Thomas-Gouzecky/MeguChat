import pytest
from mcp import Client

from app.server import mcp
import app.tools


async def test_tools(client):
    result = await client.list_tools()

    assert any(tool.name == "get_groupchats" for tool in result.tools)
