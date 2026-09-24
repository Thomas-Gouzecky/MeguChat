public interface IMessagesClient
{
    Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> SendMessageToGroupChatAsync(int groupChatId, MessageCreationRequestDto request, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMessageFromGroupChatAsync(int groupChatId, int messageId, string userId, CancellationToken cancellationToken = default);
}