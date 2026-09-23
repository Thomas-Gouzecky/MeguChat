public interface IGroupChatService
{
    Task<IEnumerable<GroupChatResponseDto>> GetCurrentUserGroupChatsAsync();
    Task<GroupChatResponseDto> CreateGroupChatAsync(CreateGroupChatRequestDto request);
}