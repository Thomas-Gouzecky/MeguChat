from mcp_types import Icon
from pydantic import Field
from server import mcp


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
    name="test-resource",
    description="This is a test resource for MeguChat. It demonstrates how to create a simple resource that can be used with the MeguChat server.",
)
def test_resource() -> str:
    """
    This is a test resource for MeguChat. It demonstrates how to create a simple resource that can be used with the MeguChat server.

    Returns:
        str: The test resource.
    """
    return "This is a test resource for MeguChat."


@mcp.prompt(
    name="test-prompt",
    description="This is a test prompt for MeguChat. It demonstrates how to create a simple prompt that can be used with the MeguChat server.",
)
def test_prompt() -> str:
    """
    This is a test prompt for MeguChat. It demonstrates how to create a simple prompt that can be used with the MeguChat server.

    Returns:
        str: The test prompt.
    """
    return "This is a test prompt for MeguChat."
