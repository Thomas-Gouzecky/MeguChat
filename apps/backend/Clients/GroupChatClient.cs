public class GroupChatClient : IGroupChatClient
{
    private readonly HttpClient _dbApiClient;

    public GroupChatClient(IHttpClientFactory httpClientFactory)
    {
        _dbApiClient = httpClientFactory.CreateClient("dbApi");
    }

    public async Task<IEnumerable<GroupChatResponseDto>> GetGroupChatsForUserAsync(string userId, CancellationToken cancellationToken)
    {
        var response = await _dbApiClient.GetAsync($"/api/groupchats/user/{userId}");
        response.EnsureSuccessStatusCode();

        var groupChats = await response.Content.ReadFromJsonAsync<IEnumerable<GroupChatResponseDto>>();
        return groupChats ?? Enumerable.Empty<GroupChatResponseDto>();
    }

    public async Task<GroupChatResponseDto> CreateGroupChatAsync(string userId, CreateGroupChatRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _dbApiClient.PostAsJsonAsync($"/api/groupchats", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var groupChat = await response.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        if (groupChat is null)
        {
            throw new InvalidOperationException("Failed to create group chat.");
        }
        return groupChat;
    }

    public async Task AddMembersToGroupChatAsync(int groupChatId, IEnumerable<string> userIds, CancellationToken cancellationToken)
    {
        var response = await _dbApiClient.PostAsJsonAsync(
            $"/api/groupchats/{groupChatId}/members",
            new { users = userIds },
            cancellationToken);
        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to add members to group chat {groupChatId}. Status code: {response.StatusCode}");
        }
    }
}