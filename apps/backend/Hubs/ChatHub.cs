using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessagesService _messagesService;
    private readonly ILogger<ChatHub> _logger;
    public ChatHub(IMessagesService messagesService, ILogger<ChatHub> logger)
    {
        _messagesService = messagesService;
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "ChatHub connection accepted: ConnectionId={ConnectionId}, UserId={UserId}, IsAuthenticated={IsAuthenticated}",
            Context.ConnectionId,
            Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown",
            Context.User?.Identity?.IsAuthenticated ?? false);

        return base.OnConnectedAsync();
    }
    public async Task JoinGroupChat(int groupchatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"groupchat:{groupchatId}");
    }

    public async Task LeaveGroupChat(int groupchatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"groupchat:{groupchatId}");
    }

    public async Task SendMessage(int groupchatId, MessageCreationRequestDto request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "ChatHub SendMessage entered: ConnectionId={ConnectionId}, GroupChatId={GroupChatId}, UserId={UserId}",
            Context.ConnectionId,
            groupchatId,
            Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown");

        try
        {
            var createdMessage = await _messagesService.SendMessageToGroupChatAsync(groupchatId, request, cancellationToken);
            await Clients.Group($"groupchat:{groupchatId}").SendAsync("ReceiveMessage", createdMessage, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to send message to group chat {GroupChatId} for user {UserId}",
                groupchatId,
                Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown");
            throw;
        }
    }
}