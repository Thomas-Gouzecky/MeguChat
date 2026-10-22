using Microsoft.Extensions.AI;

public interface IAIService
{
    Task<ChatResponse> ChatAsync(AIMessageRequest request);
    bool IsHealthy();
}