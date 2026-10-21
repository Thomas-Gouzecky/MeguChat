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

    public async Task<ChatResponse> ProcessMessageAsync(string message, string userId, string sessionId, CancellationToken cancellationToken)
    {
        var mcpClient = await GetMcpClientAsync();
        var tools = await mcpClient.ListToolsAsync();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.Assistant, "You are a helpful assistant. Please provide a concise and informative response to the user's query."),
            new(ChatRole.User, message)
        };

        var options = new ChatOptions
        {
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
            await _mcpClient.ConnectAsync("localhost", 8023);
        }

        return _mcpClient;
    }
}