public interface IMembersClient
{
    Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId, string currentUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MemberResponseDto>> AddMembersToGroupChatAsync(int groupChatId, IEnumerable<string> userIds, string currentUserId, CancellationToken cancellationToken = default);
    Task<MemberResponseDto> RemoveMemberFromGroupChatAsync(int groupChatId, string userId, string currentUserId, CancellationToken cancellationToken = default);
}