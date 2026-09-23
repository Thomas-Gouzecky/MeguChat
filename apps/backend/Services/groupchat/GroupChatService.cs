public class GroupChatService : IGroupChatService
{
    private readonly IAuthService _authService;

    public GroupChatService(IAuthService authService)
    {
        _authService = authService;
    }
}