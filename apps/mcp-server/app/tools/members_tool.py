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
