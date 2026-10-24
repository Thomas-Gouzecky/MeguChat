from types import SimpleNamespace
from typing import cast
from mcp.server.mcpserver.context import Context


def context_with_headers(headers: dict[str, str] | None) -> Context:
    return cast(Context, SimpleNamespace(headers=headers))
