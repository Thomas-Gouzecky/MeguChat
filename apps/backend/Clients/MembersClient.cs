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
}