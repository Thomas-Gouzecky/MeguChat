public interface IMessagesService
{
    Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, int limit, DateTime? before_created_at, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> SendMessageToGroupChatAsync(int groupChatId, MessageCreationRequestDto request, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> DeleteMessageByIdAsync(int groupChatId, int messageId, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> UpdateMessageByIdAsync(int groupChatId, int messageId, MessageUpdateRequestDto request, CancellationToken cancellationToken = default);
}