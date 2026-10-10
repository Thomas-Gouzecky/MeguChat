using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessagesService _messagesService;
    public ChatHub(IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }
    public async Task JoinGroupChat(string groupchatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"groupchat:{groupchatId}");
    }

    public async Task LeaveGroupChat(string groupchatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"groupchat:{groupchatId}");
    }

    public async Task SendMessage(int groupchatId, MessageCreationRequestDto request, CancellationToken cancellationToken = default)
    {
        var createdMessage = await _messagesService.SendMessageToGroupChatAsync(groupchatId, request, cancellationToken);
        await Clients.Group($"groupchat:{groupchatId}").SendAsync("ReceiveMessage", createdMessage);
    }
}