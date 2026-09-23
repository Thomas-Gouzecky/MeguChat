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
        try
        {
            var groupChats = await _groupChatService.GetCurrentUserGroupChatsAsync();
            return Ok(groupChats);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "User not authenticated",
                Detail = ex.Message,
                Status = 401
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Group chats not found",
                Detail = ex.Message,
                Status = 404
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Error retrieving group chats",
                Detail = ex.Message,
                Status = 400
            });
        }
    }
}