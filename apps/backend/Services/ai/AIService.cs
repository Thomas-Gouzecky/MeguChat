using Microsoft.Extensions.AI;

public class AIService : IAIService
{

    private readonly ILogger<AIService> _logger;
    private readonly IAIOrchestrator _aiOrchestrator;
    private readonly IUserValidation _userValidation;

    public AIService(ILogger<AIService> logger, IAIOrchestrator aiOrchestrator, IUserValidation userValidation)
    {
        _logger = logger;
        _aiOrchestrator = aiOrchestrator;
        _userValidation = userValidation;
    }

    public async Task<ChatResponse> ChatAsync(AIMessageRequest request)
    {
        var user = await _userValidation.ValidateUser();
        var userContext = new AIUserContext(user.Id, user.UserName!, user.Email!);
        return await _aiOrchestrator.ProcessMessageAsync(request.Prompt, userContext, "sessionId", CancellationToken.None);
    }

    public bool IsHealthy()
    {
        return _aiOrchestrator.isHealthy();
    }
}