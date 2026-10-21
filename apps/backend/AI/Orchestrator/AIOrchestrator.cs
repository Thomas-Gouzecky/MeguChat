using Microsoft.Extensions.AI;

public class AIOrchestrator : IAIOrchestrator
{
    private readonly IMCPClient _mcpClient;
    private readonly ILogger<AIOrchestrator> _logger;
    private readonly IChatClient _chatClient;

    public AIOrchestrator(IMCPClient mcpClient, ILogger<AIOrchestrator> logger, IChatClient chatClient)
    {
        _mcpClient = mcpClient;
        _logger = logger;
        _chatClient = chatClient;
    }

    public async Task<string> ProcessMessageAsync(string message, string userId, string sessionId, CancellationToken cancellationToken)
    {
        var mcpClient = await GetMcpClientAsync();
        var tools = await mcpClient.ListToolsAsync();

        return "Processed message: " + message + " | User ID: " + userId + " | Session ID: " + sessionId;
    }

    private async Task<IMCPClient> GetMcpClientAsync()
    {
        if (!_mcpClient.IsConnected)
        {
            await _mcpClient.ConnectAsync("localhost", 8023);
        }

        return _mcpClient;
    }
}