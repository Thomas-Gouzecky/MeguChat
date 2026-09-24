public interface IMessagesService
{
    Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> SendMessageToGroupChatAsync(int groupChatId, MessageCreationRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteMessageByIdAsync(int groupChatId, int messageId, CancellationToken cancellationToken = default);
}