public class MembersService : IMembersService
{
    private readonly IMembersClient _membersClient;

    public MembersService(IMembersClient membersClient)
    {
        _membersClient = membersClient;
    }

    public async Task<List<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId)
    {
        var members = await _membersClient.GetMembersOfGroupChatAsync(groupChatId);
        return members;
    }
}