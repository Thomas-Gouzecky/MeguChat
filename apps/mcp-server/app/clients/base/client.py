from mcp.server.mcpserver.context import Context


class BaseClient:
    def get_cookie(self, context: Context) -> str:
        incoming_headers = context.headers or {}
        cookie = incoming_headers.get("cookie")

        if not cookie:
            raise ValueError("Missing cookie in request headers.")

        return cookie
