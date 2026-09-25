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

        return groupChat;
    }

    public async Task<GroupChatResponseDto> UpdateGroupChatAsync(int groupChatId, UpdateGroupChatRequestDto request)
    {
        ValidateObject.Validate(request);

        var user = await _userValidation.ValidateUser();


        // Save the updated group chat
        var response = await _groupChatClient.UpdateGroupChatAsync(groupChatId, user.Id, request, CancellationToken.None);

        return response;
    }

    public async Task<bool> DeleteGroupChatAsync(int groupChatId)
    {
        var user = await _userValidation.ValidateUser();

        var response = await _groupChatClient.DeleteGroupChatAsync(groupChatId, user.Id, CancellationToken.None);
        return response;
    }

}