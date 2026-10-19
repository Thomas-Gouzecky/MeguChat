import json
from mcp import Client

from app.server import mcp
import app.tools

# ===============================================================
# Tools
# ===============================================================


async def test_list_tools(client):
    result = await client.list_tools()

    assert any(tool.name == "test-tool" for tool in result.tools)


async def test_test_tool(client):
    result = await client.call_tool(
        "test-tool",
        {
            "message": "Hello, world!",
        },
    )

    assert result.content[0].type == "text"
    assert result.content[0].text == "Echo: Hello, world!"


# ===============================================================
# Resources
# ===============================================================


async def test_list_resources(client):
    # returns a json object with the resource data
    result = await client.read_resource("test://test-resource")

    assert len(result.contents) == 1

    content = result.contents[0]
    data = json.loads(content.text)

    assert data == ["resource1", "resource2", "resource3"]


# ===============================================================
# Prompts
# ===============================================================
