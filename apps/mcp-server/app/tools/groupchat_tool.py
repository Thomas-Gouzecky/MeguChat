from typing import Any

from mcp_types import ToolAnnotations
from pydantic import Field

from app.server import mcp

from mcp.server.mcpserver.context import Context

from app.clients.groupchat_client import GroupchatClient

from app.DTO import CreateGroupchatDTO, UpdateGroupchatDTO

client = GroupchatClient()


@mcp.tool(
    name="create-groupchat",
    description="Create a group chat through the backend gateway.",
    annotations=ToolAnnotations(
        title="Create Groupchat",
        open_world_hint=True,
    ),
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
    annotations=ToolAnnotations(
        title="Get Groupchat by Id",
        read_only_hint=True,
        open_world_hint=True,
    ),
)
async def get_groupchat_by_id(
    ctx: Context = Field(..., description="The context of the request."),
    groupchat_id: int = Field(..., description="The ID of the group chat."),
) -> dict[str, Any]:
    result = await client.get_groupchat(ctx=ctx, groupchat_id=groupchat_id)
    return result


@mcp.tool(
    name="get-current-user-groupchats",
    description="Get all group chats for the current user.",
    annotations=ToolAnnotations(
        title="Get Current User Groupchats",
        read_only_hint=True,
        open_world_hint=True,
    ),
)
async def get_current_user_groupchats(
    ctx: Context = Field(..., description="The context of the request."),
) -> list[dict[str, Any]]:
    result = await client.get_current_user_groupchats(ctx=ctx)
    return result


@mcp.tool(
    name="update-groupchat",
    description="Update a group chat through the backend gateway.",
    annotations=ToolAnnotations(
        title="Update Groupchat",
        open_world_hint=True,
    ),
)
async def update_groupchat(
    ctx: Context = Field(..., description="The context of the request."),
    groupchat_id: int = Field(..., description="The ID of the group chat."),
    name: str | None = Field(
        default=None, description="The new name of the group chat."
    ),
) -> dict[str, Any]:
    result = await client.update_groupchat(
        ctx=ctx, groupchat_id=groupchat_id, update_data=UpdateGroupchatDTO(name=name)
    )
    return result
