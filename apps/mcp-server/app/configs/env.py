import os
from dotenv import load_dotenv

load_dotenv(".env.local")


def get_required_setting(name: str) -> str:
    value = os.getenv(name)
    if not value:
        raise RuntimeError(
            f"Missing required mcp setting: {name}. "
            "Set it in the environment or apps/mcp-server/.env."
        )
    return value


HOST = get_required_setting("MCP_HOST")
PORT = int(get_required_setting("MCP_PORT"))
BACKEND_GATEWAY_URL = get_required_setting("BACKEND_GATEWAY_URL")
