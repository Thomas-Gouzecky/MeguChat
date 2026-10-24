import httpx
from typing import Any
from mcp.server.mcpserver.context import Context
from .base.client import BaseClient

from app.configs import BACKEND_GATEWAY_URL


class GatewayClient(BaseClient):
    def __init__(self):
        self.endpoint = BACKEND_GATEWAY_URL
        super().__init__()

    async def make_request(
        self, ctx: Context, method: str, endpoint: str, data: dict[str, Any]
    ) -> dict[str, Any]:
        """
        Make a request to the backend gateway.

        Args:
            ctx (Context): The context of the request.
            method (str): The HTTP method (GET, POST, etc.).
            endpoint (str): The API endpoint to call.
            data (dict[str, Any], optional): The data to send in the request body.

        Returns:
            dict[str, any]: The response from the backend gateway.
        """
        cookie = self.get_cookie(ctx)
        async with httpx.AsyncClient() as client:
            response = await client.request(
                method,
                f"{self.endpoint}/api{endpoint}",
                json=data,
                headers={"Cookie": cookie},
            )
            response.raise_for_status()
            return response.json()
