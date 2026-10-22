using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

public class MCPClient : IMCPClient, IAsyncDisposable
{
    private McpClient? _client;
    private readonly ILogger<MCPClient> _logger;
    private readonly SemaphoreSlim _connectLock = new(1, 1);
    public bool IsConnected => _client is not null;
    public MCPClient(ILogger<MCPClient> logger)
    {
        _logger = logger;

    }

    public async Task ConnectAsync(string endpoint)
    {
        await _connectLock.WaitAsync();

        try
        {
            if (_client is not null)
            {
                return;
            }

            var transport = new HttpClientTransport(new()
            {
                Name = "MCPClient",
                Endpoint = new Uri(endpoint),
                TransportMode = HttpTransportMode.StreamableHttp
            });

            _client = await McpClient.CreateAsync(transport);

            _logger.LogInformation(
                "Connected to MCP server at {Endpoint}",
                endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to connect to MCP server at {Endpoint}",
                endpoint);
            throw;
        }
        finally
        {
            _connectLock.Release();
        }
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

        var client = _client;
        _client = null;

        await client.DisposeAsync();

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