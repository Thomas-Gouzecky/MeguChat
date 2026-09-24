public interface IMessagesService
{
    Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default);
}