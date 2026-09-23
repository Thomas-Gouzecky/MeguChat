public class GroupChatService : IGroupChatService
{
    private readonly IAuthService _authService;
    private readonly IGroupChatClient _groupChatClient;

    public GroupChatService(IAuthService authService, IGroupChatClient groupChatClient)
    {
        _authService = authService;
        _groupChatClient = groupChatClient;
    }

    public async Task<IEnumerable<GroupChatResponseDto>> GetCurrentUserGroupChatsAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user is null)
        {
            throw new UnauthenticatedAccessException("User is not authenticated.");
        }

        var groupChats = await _groupChatClient.GetGroupChatsForUserAsync(user.Id, CancellationToken.None);

        if (groupChats is null || !groupChats.Any())
        {
            throw new NotFoundException("No group chats found for the current user.");
        }
        return groupChats;
    }
}