from typing import Any

from pydantic import Field

from app.server import mcp

from mcp.server.mcpserver.context import Context

from app.clients.groupchat_client import GroupchatClient

from app.DTO import CreateGroupchatDTO

client = GroupchatClient()


@mcp.tool(
    name="create-groupchat",
    description="Create a group chat through the backend gateway.",
)
async def create_groupchat(
    ctx: Context = Field(..., description="The context of the request."),
    name: str = Field(..., description="The name of the group chat."),
    users: list[str] = Field(
        default_factory=list,
        description="The user IDs to add to the group chat.",
    ),
) -> dict[str, Any]:
    result = await client.create_groupchat(
        ctx=ctx, data=CreateGroupchatDTO(name=name, users=users)
    )
    return result


@mcp.tool(
    name="get-groupchat-by-id",
    description="Get a group chat by its Id through the backend gateway.",
)
async def get_groupchat_by_id(
    ctx: Context = Field(..., description="The context of the request."),
    groupchat_id: int = Field(..., description="The ID of the group chat."),
) -> dict[str, Any]:
    result = await client.get_groupchat(ctx=ctx, groupchat_id=groupchat_id)
    return result
