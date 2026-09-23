public class GroupChatClient : IGroupChatClient
{
    private readonly HttpClient _dbApiClient;

    public GroupChatClient(IHttpClientFactory httpClientFactory)
    {
        _dbApiClient = httpClientFactory.CreateClient("dbApi");
    }

    public async Task<IEnumerable<GroupChatResponseDto>> GetGroupChatsForUserAsync(string userId, CancellationToken cancellationToken)
    {
        var response = await _dbApiClient.GetAsync($"groupchats/user/{userId}");
        response.EnsureSuccessStatusCode();

        var groupChats = await response.Content.ReadFromJsonAsync<IEnumerable<GroupChatResponseDto>>();
        return groupChats ?? Enumerable.Empty<GroupChatResponseDto>();
    }
}