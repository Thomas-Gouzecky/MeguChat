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
        ValidateObject.Validate(request);

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

    public async Task<GroupChatResponseDto> UpdateGroupChatAsync(int groupChatId, UpdateGroupChatRequestDto request)
    {
        ValidateObject.Validate(request);

        var user = await _authService.GetCurrentUserAsync();
        if (user is null)
        {
            throw new UnauthenticatedAccessException("User is not authenticated.");
        }

        // Ensure the user is a member of the group chat before allowing updates
        var groupChats = await _groupChatClient.GetGroupChatsForUserAsync(user.Id, CancellationToken.None);
        var groupChat = groupChats.FirstOrDefault(gc => gc.Id == groupChatId);

        if (groupChat is null)
        {
            throw new NotFoundException($"Group chat with ID {groupChatId} was not found or the user is not a member of it.");
        }

        // Update the group chat
        groupChat.Name = request.Name;

        // Save the updated group chat
        await _groupChatClient.UpdateGroupChatAsync(groupChat.Id, groupChat, CancellationToken.None);

        return groupChat;
    }
}