using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("/api/groupchats/{groupChatId}/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessagesService _messagesService;

    public MessagesController(IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetMessagesOfGroupChat(int groupChatId, CancellationToken cancellationToken = default)
    {
        var messages = await _messagesService.GetMessagesOfGroupChatAsync(groupChatId, cancellationToken);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<ActionResult<MessageResponseDto>> SendMessageToGroupChat(int groupChatId, MessageCreationRequestDto request, CancellationToken cancellationToken = default)
    {
        var message = await _messagesService.SendMessageToGroupChatAsync(groupChatId, request, cancellationToken);
        return Ok(message);
    }
}