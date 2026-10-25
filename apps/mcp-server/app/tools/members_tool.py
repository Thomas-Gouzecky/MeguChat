from typing import Any

from pydantic import Field
from mcp.server.mcpserver.context import Context

from app.server import mcp
from app.clients.members_client import MembersClient

client = MembersClient()


@mcp.tool(
    name="get-members-of-groupchat",
    description="Get the members of a groupchat",
)
async def get_members_of_groupchat(
    ctx: Context = Field(..., description="The context of the request."),
    groupchat_id: int = Field(..., description="The ID of the group chat."),
) -> list[dict[str, Any]]:
    result = await client.get_members_of_groupchat(ctx=ctx, groupchat_id=groupchat_id)
    return result

@mcp.tool(
    name="add-members-to-groupchat",
    description="Add members to a groupchat",
)
async def add_members_to_groupchat(
    ctx: Context = Field(..., description="The context of the request."),
    groupchat_id: int = Field(..., description="The ID of the group chat."),
    user_ids: list[str] = Field(
        default_factory=list,
        description="The user IDs to add to the group chat.",
    ),
) -> dict[str, Any]:
    result = await client.add_members_to_groupchat(
        ctx=ctx, groupchat_id=groupchat_id, user_ids=user_ids
    )
    return result

@mcp.tool(
    name="remove-member-from-groupchat",
    description="Remove a member from a groupchat",
)
async def remove_member_from_groupchat(
    ctx: Context = Field(..., description="The context of the request."),
    groupchat_id: int = Field(..., description="The ID of the group chat."),
    user_id: str = Field(..., description="The ID of the user to remove."),
) -> dict[str, Any]:
    result = await client.remove_member_from_groupchat(
        ctx=ctx, groupchat_id=groupchat_id, user_id=user_id
    )
    return result