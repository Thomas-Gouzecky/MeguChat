import json
from mcp import Client

from app.server import mcp
import app.tools


async def test_list_tools(client):
    result = await client.list_tools()

    assert any(tool.name == "test-tool" for tool in result.tools)


async def test_list_resources(client):
    # returns a json object with the resource data
    result = await client.read_resource("test://test-resource")

    assert len(result.contents) == 1

    content = result.contents[0]
    data = json.loads(content.text)

    assert data == ["resource1", "resource2", "resource3"]
