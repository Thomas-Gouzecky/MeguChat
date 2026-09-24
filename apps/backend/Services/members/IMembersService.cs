public interface IMembersService
{
    Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId);
}