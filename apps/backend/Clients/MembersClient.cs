public class MembersClient : IMembersClient
{
    private readonly HttpClient _dbApiClient;

    public MembersClient(IHttpClientFactory httpClientFactory)
    {
        _dbApiClient = httpClientFactory.CreateClient("dbApi");
    }

    public async Task<List<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId)
    {
        var response = await _dbApiClient.GetAsync($"/api/groupchats/{groupChatId}/members");
        response.EnsureSuccessStatusCode();

        var members = await response.Content.ReadFromJsonAsync<List<MemberResponseDto>>();
        return members ?? new List<MemberResponseDto>();
    }
}