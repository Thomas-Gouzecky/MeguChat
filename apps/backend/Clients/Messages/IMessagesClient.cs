public interface IMessagesClient
{
    Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, string userId, int limit, DateTime? before_created_at, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> SendMessageToGroupChatAsync(int groupChatId, MessageCreationRequestDto request, string userId, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> UpdateMessageInGroupChatAsync(int groupChatId, int messageId, MessageUpdateRequestDto request, string userId, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> DeleteMessageFromGroupChatAsync(int groupChatId, int messageId, string userId, CancellationToken cancellationToken = default);
}