from mcp.server import MCPServer
from configs import SERVER_NAME, SERVER_VERSION, SERVER_DESCRIPTION
from tools import *

mcp = MCPServer(
    SERVER_NAME,
    version=SERVER_VERSION,
    description=SERVER_DESCRIPTION,
)
