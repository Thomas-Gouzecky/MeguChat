using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Microsoft.AspNetCore.Http;

public class MCPClient : IMCPClient, IAsyncDisposable
{
    private McpClient? _client;
    private readonly ILogger<MCPClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SemaphoreSlim _connectLock = new(1, 1);
    public bool IsConnected => _client is not null;
    public MCPClient(
        ILogger<MCPClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
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

            var cookie = _httpContextAccessor.HttpContext?.Request.Headers.Cookie.ToString();
            if (string.IsNullOrWhiteSpace(cookie))
            {
                throw new InvalidOperationException(
                    "The current request does not contain an authentication cookie.");
            }

            var transport = new HttpClientTransport(new()
            {
                Name = "MCPClient",
                Endpoint = new Uri(endpoint),
                TransportMode = HttpTransportMode.StreamableHttp,
                AdditionalHeaders = new Dictionary<string, string>
                {
                    ["Cookie"] = cookie
                }
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