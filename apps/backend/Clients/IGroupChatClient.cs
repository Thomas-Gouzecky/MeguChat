public interface IGroupChatClient
{
    Task<IEnumerable<GroupChatResponseDto>> GetGroupChatsForUserAsync(string userId, CancellationToken cancellationToken);
}