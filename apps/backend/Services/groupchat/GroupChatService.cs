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
            throw new InvalidOperationException("User is not authenticated.");
        }

        var groupChats = await _groupChatClient.GetGroupChatsForUserAsync(user.Id, CancellationToken.None);

        return groupChats;
    }
}