public class GroupChatService : IGroupChatService
{
    private readonly IGroupChatClient _groupChatClient;
    private readonly IUserValidation _userValidation;

    public GroupChatService(IGroupChatClient groupChatClient, IUserValidation userValidation)
    {
        _groupChatClient = groupChatClient;
        _userValidation = userValidation;
    }

    public async Task<IEnumerable<GroupChatResponseDto>> GetCurrentUserGroupChatsAsync()
    {
        var user = await _userValidation.ValidateUser();

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

        var user = await _userValidation.ValidateUser();

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

        var user = await _userValidation.ValidateUser();

        var groupChat = await _userValidation.EnsureUserIsMember(user.Id, groupChatId);

        // Update the group chat
        groupChat.Name = request.Name;

        // Save the updated group chat
        await _groupChatClient.UpdateGroupChatAsync(groupChat.Id, groupChat, CancellationToken.None);

        return groupChat;
    }

    public async Task<bool> DeleteGroupChatAsync(int groupChatId)
    {
        var user = await _userValidation.ValidateUser();

        var groupChat = await _userValidation.EnsureUserIsMember(user.Id, groupChatId);

        var response = await _groupChatClient.DeleteGroupChatAsync(groupChatId, CancellationToken.None);
        return response;
    }

}