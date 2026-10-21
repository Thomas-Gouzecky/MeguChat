public interface IAIOrchestrator
{
    Task<string> ProcessMessageAsync(string message, string userId, string sessionId, CancellationToken cancellationToken);
}