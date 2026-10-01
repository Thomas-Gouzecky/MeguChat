public sealed class FakeMembersClient : IMembersClient
{
    private readonly TestDataStore _store;

    public FakeMembersClient(TestDataStore store)
    {
        _store = store;
    }

    public Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(
        int groupChatId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.GetMembers(groupChatId, currentUserId));
    }

    public Task<IEnumerable<MemberResponseDto>> AddMembersToGroupChatAsync(
        int groupChatId,
        IEnumerable<string> userIds,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.AddMembers(groupChatId, userIds, currentUserId));
    }

    public Task<MemberResponseDto> RemoveMemberFromGroupChatAsync(
        int groupChatId,
        string userId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.RemoveMember(groupChatId, userId, currentUserId));
    }
}
