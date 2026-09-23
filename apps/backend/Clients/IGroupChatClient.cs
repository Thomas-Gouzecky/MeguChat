public interface IGroupChatClient
{
    Task<IEnumerable<GroupChatResponseDto>> GetGroupChatsForUserAsync(string userId, CancellationToken cancellationToken);
    Task<GroupChatResponseDto> CreateGroupChatAsync(string userId, CreateGroupChatRequestDto request, CancellationToken cancellationToken);
    Task AddMembersToGroupChatAsync(int groupChatId, IEnumerable<string> userIds, CancellationToken cancellationToken);
}