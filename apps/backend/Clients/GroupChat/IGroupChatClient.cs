public interface IGroupChatClient
{
    Task<IEnumerable<GroupChatResponseDto>> GetGroupChatsForUserAsync(string currentUserId, CancellationToken cancellationToken);
    Task<GroupChatResponseDto> CreateGroupChatAsync(string currentUserId, CreateGroupChatRequestDto request, CancellationToken cancellationToken);
    Task<GroupChatResponseDto> UpdateGroupChatAsync(int groupChatId, string currentUserId, UpdateGroupChatRequestDto groupChat, CancellationToken cancellationToken);
    Task<bool> DeleteGroupChatAsync(int groupChatId, string currentUserId, CancellationToken cancellationToken);
}