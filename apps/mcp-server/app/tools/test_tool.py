from mcp_types import Icon
from pydantic import Field
from app.server import mcp
from mcp.server.mcpserver.prompts import base

resources = ["resource1, resource2", "resource3"]


@mcp.tool(
    name="test-tool",
    icons=[
        Icon(
            src="https://avatars.githubusercontent.com/u/180672822?v=4&size=64",
        )
    ],
    description="This is a test tool for MeguChat. It demonstrates how to create a simple tool that can be used with the MeguChat server.",
)
def test_tool(
    message: str = Field(
        ...,
        description="The message to be echoed back.",
    ),
) -> str:
    """
    This is a test tool for MeguChat. It demonstrates how to create a simple tool that can be used with the MeguChat server.

    Args:
        message (str): The message to be echoed back.

    Returns:
        str: The echoed message.
    """
    return f"Echo: {message}"


@mcp.resource(
    uri="test://test-resource",
    mime_type="application/json",
    name="test-resource",
    description="This is a test resource for MeguChat. It demonstrates how to create a simple resource that can be used with the MeguChat server.",
)
def test_resource() -> list[str]:
    """
    This is a test resource for MeguChat. It demonstrates how to create a simple resource that can be used with the MeguChat server.

    Returns:
        list[str]: The test resource.
    """
    return resources


@mcp.prompt(
    name="test-prompt",
    description="This is a test prompt for MeguChat. It demonstrates how to create a simple prompt that can be used with the MeguChat server.",
)
def test_prompt(
    message: str = Field(..., description="The message to be reworded.")
) -> list[base.Message]:
    """
    This is a test prompt for MeguChat. It demonstrates how to create a simple prompt that can be used with the MeguChat server.

    Args:
        message (str): The message to be reworded.

    Returns:
        list[base.Message]: The list of messages.
    """

    prompt = f"""
    Your task is to reword the following message in a way that is more professional for MeguChat.

    The message you need to reword is:
    <message>
    {message}
    </message>

    You can add in any additional information that you think is necessary to make the message more professional, but do not remove any information from the original message.
    """

    return [base.UserMessage(role="user", content=prompt)]
