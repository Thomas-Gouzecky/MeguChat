public class MembersClient : IMembersClient
{
    private readonly HttpClient _dbApiClient;

    public MembersClient(IHttpClientFactory httpClientFactory)
    {
        _dbApiClient = httpClientFactory.CreateClient("dbApi");
    }

    public async Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default)
    {
        var response = await _dbApiClient.GetAsync($"/api/groupchats/{groupChatId}/members", cancellationToken);
        response.EnsureSuccessStatusCode();

        var members = await response.Content.ReadFromJsonAsync<List<MemberResponseDto>>();
        return members ?? new List<MemberResponseDto>();
    }

    public async Task<IEnumerable<MemberResponseDto>> AddMembersToGroupChatAsync(int groupChatId, IEnumerable<string> userIds, CancellationToken cancellationToken = default)
    {
        var response = await _dbApiClient.PostAsJsonAsync(
            $"/api/groupchats/{groupChatId}/members",
            new { users = userIds },
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var addedMembers = await response.Content.ReadFromJsonAsync<List<MemberResponseDto>>();
        return addedMembers ?? new List<MemberResponseDto>();
    }
}