using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("/api/groupchats/{groupChatId}/members")]
public class MembersController : ControllerBase
{
    private readonly IMembersService _membersService;

    public MembersController(IMembersService membersService)
    {
        _membersService = membersService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetMembersOfGroupChat(int groupChatId)
    {
        var members = await _membersService.GetMembersOfGroupChatAsync(groupChatId);
        return Ok(members);
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<MemberResponseDto>>> AddMemberToGroupChat(int groupChatId, [FromBody] AddMemberRequestDto request)
    {
        var result = await _membersService.AddMembersToGroupChatAsync(groupChatId, request);
        return Ok(result);
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> RemoveMemberFromGroupChat(int groupChatId, string userId)
    {
        var result = await _membersService.RemoveMemberFromGroupChatAsync(groupChatId, userId);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}