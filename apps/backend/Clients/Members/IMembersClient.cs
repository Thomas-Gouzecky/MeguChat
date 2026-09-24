public interface IMembersClient
{
    Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MemberResponseDto>> AddMembersToGroupChatAsync(int groupChatId, IEnumerable<string> userIds, CancellationToken cancellationToken = default);
    Task<bool> RemoveMemberFromGroupChatAsync(int groupChatId, string userId, CancellationToken cancellationToken = default);
}