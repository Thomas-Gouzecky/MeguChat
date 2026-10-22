using Microsoft.Extensions.AI;

public interface IAIOrchestrator
{
    Task<ChatResponse> ProcessMessageAsync(string message, string userId, string sessionId, CancellationToken cancellationToken);
    bool isHealthy();
}