public interface IMembersService
{
    Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId);
    Task<IEnumerable<MemberResponseDto>> AddMembersToGroupChatAsync(int groupChatId, AddMemberRequestDto request);
}