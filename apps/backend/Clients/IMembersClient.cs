public interface IMembersClient
{
    Task<List<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId);
}