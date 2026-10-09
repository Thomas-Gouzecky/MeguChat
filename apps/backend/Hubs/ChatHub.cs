using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ChatHub : Hub
{
    public async Task JoinGroupChat(string groupchatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"groupchat:{groupchatId}");
    }

    public async Task LeaveGroupChat(string groupchatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"groupchat:{groupchatId}");
    }
}