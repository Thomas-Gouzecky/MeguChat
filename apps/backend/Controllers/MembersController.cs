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
    public async Task<ActionResult<List<MemberResponseDto>>> GetMembersOfGroupChat(int groupChatId)
    {
        var members = await _membersService.GetMembersOfGroupChatAsync(groupChatId);
        return Ok(members);
    }
}