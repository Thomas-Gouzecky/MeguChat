public interface IGroupChatService
{
    Task<IEnumerable<GroupChatResponseDto>> GetCurrentUserGroupChatsAsync();
    Task<GroupChatResponseDto> CreateGroupChatAsync(CreateGroupChatRequestDto request);
    Task<GroupChatResponseDto> UpdateGroupChatAsync(int groupChatId, UpdateGroupChatRequestDto request);
    Task<bool> DeleteGroupChatAsync(int groupChatId);
}