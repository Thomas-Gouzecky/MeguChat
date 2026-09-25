public class UserValidation : IUserValidation
{
    private readonly IAuthService _authService;
    private readonly IGroupChatClient _groupChatClient;
    public UserValidation(IAuthService authService, IGroupChatClient groupChatClient)
    {
        _authService = authService;
        _groupChatClient = groupChatClient;
    }
    public static bool IsValidUserId(string userId)
    {
        // Check if the userId is not null or empty
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        // Check if the userId is a valid GUID
        return Guid.TryParse(userId, out _);
    }

    // public async Task<GroupChatResponseDto> EnsureUserIsMember(string userId, int groupChatId)
    // {
    //     // Ensure the user is a member of the group chat before allowing updates
    //     var groupChats = await _groupChatClient.GetGroupChatsForUserAsync(userId, CancellationToken.None);
    //     var groupChat = groupChats.FirstOrDefault(gc => gc.Id == groupChatId);

    //     if (groupChat is null)
    //     {
    //         throw new NotFoundException($"Group chat with ID {groupChatId} was not found or the user is not a member of it.");
    //     }

    //     return groupChat;
    // }

    public async Task<ApplicationUser> ValidateUser()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user is null)
        {
            throw new UnauthenticatedAccessException("User is not authenticated.");
        }

        return user;
    }
}