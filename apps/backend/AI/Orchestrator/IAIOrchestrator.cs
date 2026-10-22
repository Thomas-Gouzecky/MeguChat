using Microsoft.Extensions.AI;

public interface IAIOrchestrator
{
    Task<ChatResponse> ProcessMessageAsync(string message, AIUserContext currentUserContext, string sessionId, CancellationToken cancellationToken);
    bool isHealthy();
}