using Microsoft.Extensions.AI;

public class AIOrchestrator : IAIOrchestrator
{
    private readonly IMCPClient _mcpClient;
    private readonly ILogger<AIOrchestrator> _logger;
    private readonly IChatClient _chatClient;
    private readonly IConfiguration _configuration;

    public AIOrchestrator(IMCPClient mcpClient, ILogger<AIOrchestrator> logger, IChatClient chatClient, IConfiguration configuration)
    {
        _mcpClient = mcpClient;
        _logger = logger;
        _chatClient = chatClient;
        _configuration = configuration;
    }

    public async Task<ChatResponse> ProcessMessageAsync(string message, string userId, string sessionId, CancellationToken cancellationToken)
    {
        var mcpClient = await GetMcpClientAsync();
        var tools = await mcpClient.ListToolsAsync();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, message)
        };

        var options = new ChatOptions
        {
            Instructions = $"""
                You are MeguAI, a helpful assistant for MeguChat.

                Current user ID: {userId}
                Current session ID: {sessionId}

                Be concise and informative.
                Use available tools when appropriate.
                """,
            Tools = [.. tools]
        };

        var response = await _chatClient.GetResponseAsync(messages, options, cancellationToken);

        _logger.LogInformation("AI response received.");
        return response;
    }

    private async Task<IMCPClient> GetMcpClientAsync()
    {
        if (!_mcpClient.IsConnected)
        {
            var host = _configuration["MCP_HOST"]
                ?? throw new InvalidOperationException(
                    "MCP_HOST environment variable is missing.");

            var portString = _configuration["MCP_PORT"]
                ?? throw new InvalidOperationException(
                    "MCP_PORT environment variable is missing.");

            if (!int.TryParse(portString, out var port))
            {
                throw new InvalidOperationException(
                    "MCP_PORT must be a valid integer.");
            }
            await _mcpClient.ConnectAsync(host, port);
        }

        return _mcpClient;
    }
}