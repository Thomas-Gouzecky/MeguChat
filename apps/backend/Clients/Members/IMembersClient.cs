public interface IMembersClient
{
    Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MemberResponseDto>> AddMembersToGroupChatAsync(int groupChatId, IEnumerable<string> userIds, string currentUserId, CancellationToken cancellationToken = default);
    Task<bool> RemoveMemberFromGroupChatAsync(int groupChatId, string userId, string currentUserId, CancellationToken cancellationToken = default);
}