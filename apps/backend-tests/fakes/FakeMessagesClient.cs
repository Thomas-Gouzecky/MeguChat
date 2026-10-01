public sealed class FakeMessagesClient : IMessagesClient
{
    private readonly TestDataStore _store;

    public FakeMessagesClient(TestDataStore store)
    {
        _store = store;
    }

    public Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(
        int groupChatId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.GetMessages(groupChatId, userId));
    }

    public Task<MessageResponseDto> SendMessageToGroupChatAsync(
        int groupChatId,
        MessageCreationRequestDto request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.AddMessage(groupChatId, request.Content, userId));
    }

    public Task<MessageResponseDto> DeleteMessageFromGroupChatAsync(
        int groupChatId,
        int messageId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.DeleteMessage(groupChatId, messageId, userId));
    }

    public Task<MessageResponseDto> UpdateMessageInGroupChatAsync(
        int groupChatId,
        int messageId,
        MessageUpdateRequestDto request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.UpdateMessage(groupChatId, messageId, request, userId));
    }
}
