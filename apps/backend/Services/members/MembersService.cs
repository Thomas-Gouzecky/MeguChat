public class MembersService : IMembersService
{
    private readonly IMembersClient _membersClient;
    private readonly IUserValidation _userValidation;

    public MembersService(IMembersClient membersClient, IUserValidation userValidation)
    {
        _membersClient = membersClient;
        _userValidation = userValidation;
    }

    public async Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId)
    {
        var user = await _userValidation.ValidateUser();
        await _userValidation.EnsureUserIsMember(user.Id, groupChatId);
        
        var members = await _membersClient.GetMembersOfGroupChatAsync(groupChatId);
        return members;
    }
}