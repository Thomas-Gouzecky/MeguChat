public interface IMembersClient
{
    Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default);
}