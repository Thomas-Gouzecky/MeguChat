from typing import Any

from .gateway_client import GatewayClient
from mcp.server.mcpserver.context import Context


class MembersClient(GatewayClient):
    def __init__(self):
        super().__init__()

    async def get_members_of_groupchat(
        self, ctx: Context, groupchat_id: int
    ) -> list[dict[str, Any]]:
        """
        Get the members of a group chat through the backend gateway.

        Args:
            ctx (Context): The context of the request.
            groupchat_id (int): The ID of the group chat.

        Returns:
            list[dict[str, any]]: The response from the backend gateway.
        """
        result = await self.make_request(
            ctx=ctx,
            method="GET",
            endpoint=f"/groupchats/{groupchat_id}/members",
            data={},
        )
        return result

    async def add_member_to_groupchat(
        self, ctx: Context, groupchat_id: int, user_ids: list[str]
    ) -> dict[str, Any]:
        """
        Add a member to a group chat through the backend gateway.

        Args:
            ctx (Context): The context of the request.
            groupchat_id (int): The ID of the group chat.
            user_ids (list[str]): The IDs of the users to add.

        Returns:
            dict[str, any]: The response from the backend gateway.
        """
        result = await self.make_request(
            ctx=ctx,
            method="POST",
            endpoint=f"/groupchats/{groupchat_id}/members",
            data={"userIds": user_ids},
        )
        return result

    async def remove_member_from_groupchat(
        self, ctx: Context, groupchat_id: int, user_id: str
    ) -> dict[str, Any]:
        """
        Remove a member from a group chat through the backend gateway.

        Args:
            ctx (Context): The context of the request.
            groupchat_id (int): The ID of the group chat.
            user_id (str): The ID of the user to remove.

        Returns:
            dict[str, any]: The response from the backend gateway.
        """
        result = await self.make_request(
            ctx=ctx,
            method="DELETE",
            endpoint=f"/groupchats/{groupchat_id}/members/{user_id}",
            data={},
        )
        return result
