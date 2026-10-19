from mcp.server import MCPServer
from app.configs import SERVER_NAME, SERVER_VERSION, SERVER_DESCRIPTION

mcp = MCPServer(
    SERVER_NAME,
    version=SERVER_VERSION,
    description=SERVER_DESCRIPTION,
)
