public interface IMessagesClient
{
    Task<List<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId);
}