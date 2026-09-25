public class GroupChatClient : DatabaseClient, IGroupChatClient
{
    public GroupChatClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<IEnumerable<GroupChatResponseDto>> GetGroupChatsForUserAsync(string currentUserId, CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/groupchats/user/{currentUserId}"
        );

        httpRequest.Headers.Add("X-User-ID", currentUserId);

        var response = await _dbApiClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var groupChats = await response.Content.ReadFromJsonAsync<IEnumerable<GroupChatResponseDto>>();
        return groupChats ?? Enumerable.Empty<GroupChatResponseDto>();
    }

    public async Task<GroupChatResponseDto> CreateGroupChatAsync(string currentUserId, CreateGroupChatRequestDto request, CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/groupchats"
        );

        httpRequest.Headers.Add("X-User-ID", currentUserId);

        httpRequest.Content = JsonContent.Create(request);

        var response = await _dbApiClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var groupChat = await response.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        if (groupChat is null)
        {
            throw new InvalidOperationException("Failed to create group chat.");
        }
        return groupChat;
    }

    public async Task<GroupChatResponseDto> UpdateGroupChatAsync(int groupChatId, string currentUserId, UpdateGroupChatRequestDto groupChat, CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/groupchats/{groupChatId}"
        );

        httpRequest.Headers.Add("X-User-ID", currentUserId);

        httpRequest.Content = JsonContent.Create(groupChat);

        var response = await _dbApiClient.SendAsync(httpRequest, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Group chat with ID {groupChatId} not found.");
        }
        response.EnsureSuccessStatusCode();

        var updatedGroupChat = await response.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        return updatedGroupChat ?? throw new InvalidOperationException("Failed to update group chat.");
    }

    public async Task<bool> DeleteGroupChatAsync(int groupChatId, string currentUserId, CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/groupchats/{groupChatId}"
        );

        httpRequest.Headers.Add("X-User-ID", currentUserId);

        var response = await _dbApiClient.SendAsync(httpRequest, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Group chat with ID {groupChatId} not found.");
        }
        response.EnsureSuccessStatusCode();

        return response.IsSuccessStatusCode;
    }
}