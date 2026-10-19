from server import mcp
from configs import HOST, PORT
import tools

if __name__ == "__main__":
    mcp.run(
        transport="streamable-http",
        host=HOST,
        port=PORT,
    )
