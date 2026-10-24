from typing import Any

import httpx
from pydantic import Field

from app.configs import BACKEND_GATEWAY_URL
from app.server import mcp

from mcp.server.mcpserver.context import Context


@mcp.tool(
    name="create-groupchat",
    description="Create a group chat through the backend gateway.",
)
async def create_groupchat(
    ctx: Context,
    name: str = Field(..., description="The name of the group chat."),
    users: list[str] = Field(
        default_factory=list,
        description="The user IDs to add to the group chat.",
    ),
) -> dict[str, Any]:
    incoming_headers = ctx.headers or {}
    cookie = incoming_headers.get("cookie")

    if not cookie:
        raise ValueError("Missing cookie in request headers.")

    async with httpx.AsyncClient() as client:
        response = await client.post(
            f"{BACKEND_GATEWAY_URL}/api/groupchats",
            headers={"Cookie": cookie},
            json={"name": name, "users": users},
        )
        response.raise_for_status()
        return response.json()
