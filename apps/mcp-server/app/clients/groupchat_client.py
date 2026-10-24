from typing import Any

from .gateway_client import GatewayClient
from mcp.server.mcpserver.context import Context


class GroupchatClient(GatewayClient):

    async def create_groupchat(
        self, ctx: Context, name: str, users: list[str]
    ) -> dict[str, Any]:
        """
        Create a group chat through the backend gateway.

        Args:
            ctx (Context): The context of the request.
            name (str): The name of the group chat.
            users (list[str]): The user IDs to add to the group chat.

        Returns:
            dict[str, any]: The response from the backend gateway.
        """
        result = await self.make_request(
            ctx=ctx,
            method="POST",
            endpoint="/groupchat",
            data={"name": name, "users": users},
        )
        return result
