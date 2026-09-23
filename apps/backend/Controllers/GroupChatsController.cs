using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("/api/groupchats")]
public class GroupChatsController : ControllerBase
{
    private readonly IGroupChatService _groupChatService;
    private readonly IAuthService _authService;
    private readonly HttpClient _dbApiClient;

    public GroupChatsController(IGroupChatService groupChatService, IAuthService authService, IHttpClientFactory httpClientFactory)
    {
        _groupChatService = groupChatService;
        _authService = authService;
        _dbApiClient = httpClientFactory.CreateClient("dbApi");
    }
}