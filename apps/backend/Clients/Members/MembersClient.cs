public class MembersClient : DatabaseClient, IMembersClient
{
    public MembersClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<IEnumerable<MemberResponseDto>> GetMembersOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default)
    {
        var response = await _dbApiClient.GetAsync($"/api/groupchats/{groupChatId}/members", cancellationToken);
        response.EnsureSuccessStatusCode();

        var members = await response.Content.ReadFromJsonAsync<List<MemberResponseDto>>();
        return members ?? new List<MemberResponseDto>();
    }

    public async Task<IEnumerable<MemberResponseDto>> AddMembersToGroupChatAsync(int groupChatId, IEnumerable<string> userIds, string currentUserId, CancellationToken cancellationToken = default)
    {

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/groupchats/{groupChatId}/members"
        );

        httpRequest.Headers.Add("X-User-ID", currentUserId);

        httpRequest.Content = JsonContent.Create(new { users = userIds });

        var response = await _dbApiClient.SendAsync(
            httpRequest,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        var addedMembers = await response.Content.ReadFromJsonAsync<List<MemberResponseDto>>();
        return addedMembers ?? new List<MemberResponseDto>();
    }

    public async Task<bool> RemoveMemberFromGroupChatAsync(int groupChatId, string userId, string currentUserId, CancellationToken cancellationToken = default)
    {

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/groupchats/{groupChatId}/members/{userId}"
        );

        httpRequest.Headers.Add("X-User-ID", currentUserId);

        var response = await _dbApiClient.SendAsync(
            httpRequest,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false; // Member not found
        }
        return true;
    }
}