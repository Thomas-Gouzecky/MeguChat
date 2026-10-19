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


async def test_test_prompt(client):
    message = "hey can u send me the report"

    result = await client.get_prompt(
        "test-prompt",
        {"message": message},
    )

    assert result.messages[0].content.type == "text"
    prompt_text = result.messages[0].content.text

    assert message in prompt_text
    assert "more professional for MeguChat" in prompt_text
    assert "do not remove any information" in prompt_text
