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

    [HttpDelete("{messageId}")]
    public async Task<ActionResult> DeleteMessage(int groupChatId, int messageId, CancellationToken cancellationToken = default)
    {
        var isDeleted = await _messagesService.DeleteMessageByIdAsync(groupChatId, messageId, cancellationToken);

        if (!isDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{messageId}")]
    public async Task<ActionResult<MessageResponseDto>> UpdateMessage(int groupChatId, int messageId, MessageUpdateRequestDto request, CancellationToken cancellationToken = default)
    {
        var message = await _messagesService.UpdateMessageByIdAsync(groupChatId, messageId, request, cancellationToken);

        return Ok(message);
    }
}