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

    public async Task<GroupChatResponseDto> CreateGroupChatAsync(CreateGroupChatRequestDto request)
    {
        if (request.UserIds == null || !request.UserIds.Any())
        {
            throw new ArgumentException("At least one user ID must be provided to create a group chat.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Group chat name cannot be empty.");
        }

        var user = await _authService.GetCurrentUserAsync();
        if (user is null)
        {
            throw new UnauthenticatedAccessException("User is not authenticated.");
        }

        var groupChat = await _groupChatClient.CreateGroupChatAsync(user.Id, request, CancellationToken.None);

        if (groupChat is null)
        {
            throw new InvalidOperationException("Failed to create group chat.");
        }

        // Add members to the group chat (including the creator)
        await _groupChatClient.AddMembersToGroupChatAsync(groupChat.Id, new[] { user.Id }, CancellationToken.None);
        await _groupChatClient.AddMembersToGroupChatAsync(groupChat.Id, request.UserIds, CancellationToken.None);

        return groupChat;
    }
}