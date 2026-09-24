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

    public async Task<IEnumerable<MemberResponseDto>> AddMembersToGroupChatAsync(int groupChatId, AddMemberRequestDto request)
    {

        var user = await _userValidation.ValidateUser();
        await _userValidation.EnsureUserIsMember(user.Id, groupChatId);

        if (request.UserId == null || !request.UserId.Any())
        {
            throw new ArgumentException("UserId list cannot be null or empty.", nameof(request.UserId));
        }

        var addedMembers = await _membersClient.AddMembersToGroupChatAsync(groupChatId, request.UserId);
        return addedMembers;
    }

    public async Task<bool> RemoveMemberFromGroupChatAsync(int groupChatId, string userId)
    {
        var user = await _userValidation.ValidateUser();
        await _userValidation.EnsureUserIsMember(user.Id, groupChatId);

        var result = await _membersClient.RemoveMemberFromGroupChatAsync(groupChatId, userId);
        return result;
    }
}