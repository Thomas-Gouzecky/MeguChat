using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

public class MCPClient : IMCPClient, IAsyncDisposable
{
    private McpClient? _client;
    private readonly ILogger<MCPClient> _logger;
    public MCPClient(ILogger<MCPClient> logger)
    {
        _logger = logger;

    }
    public async Task ConnectAsync(string host, int port)
    {

        if (_client is not null)
        {
            throw new InvalidOperationException("Client is already connected.");
        }

        var transport = new HttpClientTransport(new()
        {
            Name = "MCPClient",
            Endpoint = new Uri($"http://{host}:{port}/mcp"),
            TransportMode = HttpTransportMode.StreamableHttp
        });
        _client = await McpClient.CreateAsync(transport);

        _logger.LogInformation("Connected to MCP server at {Host}:{Port}", host, port);
    }

    public async ValueTask<IList<McpClientTool>> ListToolsAsync()
    {
        return await GetClient().ListToolsAsync();
    }

    public async ValueTask<CallToolResult> CallToolAsync(string toolName, Dictionary<string, object?> toolInputs)
    {
        return await GetClient().CallToolAsync(toolName, toolInputs);
    }

    public async ValueTask DisconnectAsync()
    {

        if (_client is null)
        {
            return; // Already disconnected, no action needed.
        }

        _logger.LogInformation("Disconnecting from MCP server...");

        await GetClient().DisposeAsync();
        _client = null;

        _logger.LogInformation("Disconnected from MCP server.");
    }

    private McpClient GetClient()
    {
        return _client
            ?? throw new InvalidOperationException(
                "The MCP client is not connected.");
    }
    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}