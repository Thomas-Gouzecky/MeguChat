public sealed class FakeGroupChatClient : IGroupChatClient
{
    private readonly TestDataStore _store;

    public FakeGroupChatClient(TestDataStore store)
    {
        _store = store;
    }

    public Task<IEnumerable<GroupChatResponseDto>> GetGroupChatsForUserAsync(
        string currentUserId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_store.GetGroupChatsForUser(currentUserId));
    }

    public Task<GroupChatResponseDto> GetGroupChatByIdAsync(
        int groupChatId,
        string currentUserId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_store.GetGroupChat(groupChatId, currentUserId)!);
    }

    public Task<GroupChatResponseDto> CreateGroupChatAsync(
        string currentUserId,
        CreateGroupChatRequestDto request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_store.CreateGroupChat(currentUserId, request));
    }

    public Task<GroupChatResponseDto> UpdateGroupChatAsync(
        int groupChatId,
        string currentUserId,
        UpdateGroupChatRequestDto request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_store.UpdateGroupChat(groupChatId, currentUserId, request));
    }

    public Task<GroupChatResponseDto> DeleteGroupChatAsync(
        int groupChatId,
        string currentUserId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_store.DeleteGroupChat(groupChatId, currentUserId));
    }
}
