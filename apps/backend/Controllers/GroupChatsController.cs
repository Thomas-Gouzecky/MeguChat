using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("/api/groupchats")]
public class GroupChatsController : ControllerBase
{
    private readonly IGroupChatService _groupChatService;

    public GroupChatsController(IGroupChatService groupChatService)
    {
        _groupChatService = groupChatService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCurrentUserGroupChats()
    {

        var groupChats = await _groupChatService.GetCurrentUserGroupChatsAsync();
        return Ok(groupChats);
    }
}