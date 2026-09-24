public class GroupChatClient : DatabaseClient, IGroupChatClient
{
    public GroupChatClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
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

    public async Task<GroupChatResponseDto> UpdateGroupChatAsync(int groupChatId, GroupChatResponseDto groupChat, CancellationToken cancellationToken)
    {
        var response = await _dbApiClient.PutAsJsonAsync($"/api/groupchats/{groupChatId}", groupChat, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Group chat with ID {groupChatId} not found.");
        }
        response.EnsureSuccessStatusCode();

        var updatedGroupChat = await response.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        return updatedGroupChat ?? throw new InvalidOperationException("Failed to update group chat.");
    }

    public async Task<bool> DeleteGroupChatAsync(int groupChatId, CancellationToken cancellationToken)
    {
        var response = await _dbApiClient.DeleteAsync($"/api/groupchats/{groupChatId}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Group chat with ID {groupChatId} not found.");
        }
        response.EnsureSuccessStatusCode();

        return response.IsSuccessStatusCode;
    }
}