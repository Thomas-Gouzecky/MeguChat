public interface IGroupChatService
{
    Task<IEnumerable<GroupChatResponseDto>> GetCurrentUserGroupChatsAsync();
    Task<GroupChatResponseDto> GetGroupChatByIdAsync(int groupChatId);
    Task<GroupChatResponseDto> CreateGroupChatAsync(CreateGroupChatRequestDto request);
    Task<GroupChatResponseDto> UpdateGroupChatAsync(int groupChatId, UpdateGroupChatRequestDto request);
    Task<GroupChatResponseDto> DeleteGroupChatAsync(int groupChatId);
}