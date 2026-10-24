import pytest
from mcp import Client
from app.server import mcp


@pytest.fixture
async def client(anyio_backend):
    async with Client(mcp) as client:
        yield client


@pytest.fixture
def context_with_headers(headers: dict[str, str] | None):
    from types import SimpleNamespace
    from typing import cast
    from mcp.server.mcpserver.context import Context

    return cast(Context, SimpleNamespace(headers=headers))
