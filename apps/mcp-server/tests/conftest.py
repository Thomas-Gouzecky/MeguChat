import pytest
from mcp import Client
from app.server import mcp


@pytest.fixture
async def client():
    async with Client(mcp) as client:
        yield client
