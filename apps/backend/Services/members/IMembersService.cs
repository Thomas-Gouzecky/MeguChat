public interface IMembersService
{
    Task<List<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId);
}