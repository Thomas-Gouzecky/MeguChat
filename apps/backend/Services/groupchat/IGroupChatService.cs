public interface IGroupChatService
{
    Task<IEnumerable<GroupChatResponseDto>> GetCurrentUserGroupChatsAsync();
    Task<GroupChatResponseDto> CreateGroupChatAsync(CreateGroupChatRequestDto request);
    Task<GroupChatResponseDto?> GetCurrentUserGroupChatByIdAsync(int groupChatId);
}