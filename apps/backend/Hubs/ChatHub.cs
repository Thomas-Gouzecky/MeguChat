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
        await Clients.Group($"groupchat:{groupchatId}").SendAsync("JoinGroupChat", Context.UserIdentifier, Context.ConnectionAborted);
        Context.Items[$"groupchat:{groupchatId}"] = true;
    }

    public async Task LeaveGroupChat(int groupchatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"groupchat:{groupchatId}");
        await Clients.Group($"groupchat:{groupchatId}").SendAsync("LeaveGroupChat", Context.UserIdentifier, Context.ConnectionAborted);
        Context.Items.Remove($"groupchat:{groupchatId}");
    }

    public async Task SendMessage(int groupchatId, MessageCreationRequestDto request)
    {
        RequireJoinedGroupChat(groupchatId);

        _logger.LogInformation(
            "ChatHub SendMessage entered: ConnectionId={ConnectionId}, GroupChatId={GroupChatId}, UserId={UserId}",
            Context.ConnectionId,
            groupchatId,
            Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown");

        try
        {
            var createdMessage = await _messagesService.SendMessageToGroupChatAsync(groupchatId, request, Context.ConnectionAborted);
            await Clients.Group($"groupchat:{groupchatId}").SendAsync("ReceiveMessage", createdMessage, Context.ConnectionAborted);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to send message to group chat {GroupChatId} for user {UserId}",
                groupchatId,
                Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown");
            throw new HubException(exception.Message);
        }
    }

    public async Task DeleteMessage(int groupchatId, int messageId)
    {
        RequireJoinedGroupChat(groupchatId);
        try
        {
            var deletedMessage = await _messagesService.DeleteMessageByIdAsync(groupchatId, messageId, Context.ConnectionAborted);
            if (deletedMessage is null)
            {
                throw new HubException($"Message with ID {messageId} not found in group chat {groupchatId}.");
            }
            await Clients.Group($"groupchat:{groupchatId}").SendAsync("MessageDeleted", deletedMessage, Context.ConnectionAborted);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to delete message {MessageId} from group chat {GroupChatId} for user {UserId}",
                messageId,
                groupchatId,
                Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown");
            throw new HubException(exception.Message);
        }
    }

    public async Task UpdateMessage(int groupchatId, int messageId, MessageUpdateRequestDto request)
    {
        RequireJoinedGroupChat(groupchatId);
        try
        {
            var updatedMessage = await _messagesService.UpdateMessageByIdAsync(groupchatId, messageId, request, Context.ConnectionAborted);
            if (updatedMessage is null)
            {
                throw new HubException($"Message with ID {messageId} not found in group chat {groupchatId}.");
            }
            await Clients.Group($"groupchat:{groupchatId}").SendAsync("MessageUpdated", updatedMessage, Context.ConnectionAborted);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to update message {MessageId} in group chat {GroupChatId} for user {UserId}",
                messageId,
                groupchatId,
                Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown");
            throw new HubException(exception.Message);
        }
    }

    public async Task SendTypingNotification(int groupchatId)
    {
        RequireJoinedGroupChat(groupchatId);
        await Clients.Group($"groupchat:{groupchatId}").SendAsync("UserTyping", Context.UserIdentifier, Context.ConnectionAborted);
    }

    public async Task SendStopTypingNotification(int groupchatId)
    {
        RequireJoinedGroupChat(groupchatId);
        await Clients.Group($"groupchat:{groupchatId}").SendAsync("UserStoppedTyping", Context.UserIdentifier, Context.ConnectionAborted);
    }

    private bool HasJoinedGroupChat(int groupchatId)
    {
        return Context.Items.ContainsKey($"groupchat:{groupchatId}");
    }

    private void RequireJoinedGroupChat(int groupchatId)
    {
        if (!HasJoinedGroupChat(groupchatId))
        {
            throw new HubException(
                "You must join the group chat before performing this action.");
        }
    }
}