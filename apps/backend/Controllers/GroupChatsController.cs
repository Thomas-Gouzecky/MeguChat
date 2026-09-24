using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("/api/groupchats")]
public class GroupChatsController : ControllerBase
{
    private readonly IGroupChatService _groupChatService;

    public GroupChatsController(IGroupChatService groupChatService)
    {
        _groupChatService = groupChatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserGroupChats()
    {

        var groupChats = await _groupChatService.GetCurrentUserGroupChatsAsync();
        return Ok(groupChats);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroupChat([FromBody] CreateGroupChatRequestDto request)
    {
        var groupChat = await _groupChatService.CreateGroupChatAsync(request);
        return CreatedAtAction(nameof(GetCurrentUserGroupChats), new { id = groupChat.Id }, groupChat);
    }

    [HttpGet("{groupChatId}")]
    public async Task<IActionResult> GetGroupChatById(int groupChatId)
    {
        var groupChats = await _groupChatService.GetCurrentUserGroupChatsAsync();
        var groupChat = groupChats.FirstOrDefault(gc => gc.Id == groupChatId);

        if (groupChat is null)
        {
            return NotFound();
        }

        return Ok(groupChat);
    }

    [HttpPut("{groupChatId}")]
    public async Task<IActionResult> UpdateGroupChat(int groupChatId, [FromBody] UpdateGroupChatRequestDto request)
    {
        var groupChat = await _groupChatService.UpdateGroupChatAsync(groupChatId, request);
        if (groupChat is null)
        {
            return NotFound();
        }

        return Ok(groupChat);
    }

    [HttpDelete("{groupChatId}")]
    public async Task<IActionResult> DeleteGroupChat(int groupChatId)
    {
        var isDeleted = await _groupChatService.DeleteGroupChatAsync(groupChatId);
        if (!isDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}