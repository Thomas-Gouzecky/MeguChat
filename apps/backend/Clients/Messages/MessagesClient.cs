public class MessagesClient : DatabaseClient, IMessagesClient
{
    public MessagesClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default)
    {
        var response = await _dbApiClient.GetAsync($"/api/groupchats/{groupChatId}/messages", cancellationToken);
        response.EnsureSuccessStatusCode();

        var messages = await response.Content.ReadFromJsonAsync<List<MessageResponseDto>>();
        return messages ?? new List<MessageResponseDto>();
    }
}