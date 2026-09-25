public interface IMembersClient
{
    Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId, string currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> AddMembersToGroupChatAsync(int groupChatId, IEnumerable<string> userIds, string currentUserId, CancellationToken cancellationToken = default);
    Task<bool> RemoveMemberFromGroupChatAsync(int groupChatId, string userId, string currentUserId, CancellationToken cancellationToken = default);
}