public interface IGroupChatService
{
    Task<IEnumerable<GroupChatResponseDto>> GetCurrentUserGroupChatsAsync();
}