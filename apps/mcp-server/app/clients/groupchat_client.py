from typing import Any

from .gateway_client import GatewayClient
from mcp.server.mcpserver.context import Context
from app.DTO import UpdateGroupchatDTO, CreateGroupchatDTO

from dataclasses import asdict


class GroupchatClient(GatewayClient):

    async def create_groupchat(
        self, ctx: Context, data: CreateGroupchatDTO
    ) -> dict[str, Any]:
        """
        Create a group chat through the backend gateway.

        Args:
            ctx (Context): The context of the request.
            data (CreateGroupchatDTO): The data for creating the group chat.

        Returns:
            dict[str, any]: The response from the backend gateway.
        """
        result = await self.make_request(
            ctx=ctx,
            method="POST",
            endpoint="/groupchats",
            data=asdict(data),
        )
        return result

    async def get_groupchat(self, ctx: Context, groupchat_id: int) -> dict[str, Any]:
        """
        Get a group chat through the backend gateway.

        Args:
            ctx (Context): The context of the request.
            groupchat_id (int): The ID of the group chat.

        Returns:
            dict[str, any]: The response from the backend gateway.
        """
        result = await self.make_request(
            ctx=ctx,
            method="GET",
            endpoint=f"/groupchats/{groupchat_id}",
            data={},
        )
        return result

    async def get_current_user_groupchats(self, ctx: Context) -> list[dict[str, Any]]:
        """
        Get the current user's group chats through the backend gateway.

        Args:
            ctx (Context): The context of the request.

        Returns:
            list[dict[str, any]]: The response from the backend gateway.
        """
        result = await self.make_request(
            ctx=ctx,
            method="GET",
            endpoint="/groupchats",
            data={},
        )
        return result

    async def update_groupchat(
        self, ctx: Context, groupchat_id: int, update_data: UpdateGroupchatDTO
    ) -> dict[str, Any]:
        """
        Update a group chat through the backend gateway.

        Args:
            ctx (Context): The context of the request.
            groupchat_id (int): The ID of the group chat.
            name (str, optional): The new name of the group chat.

        Returns:
            dict[str, any]: The response from the backend gateway.
        """

        result = await self.make_request(
            ctx=ctx,
            method="PUT",
            endpoint=f"/groupchats/{groupchat_id}",
            data=asdict(update_data),
        )
        return result
