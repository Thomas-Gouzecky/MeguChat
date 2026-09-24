using System.Net.Http.Json;
using System.Text;

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

    public async Task<MessageResponseDto> SendMessageToGroupChatAsync(int groupChatId, MessageCreationRequestDto request, string userId, CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/groupchats/{groupChatId}/messages"
        );

        httpRequest.Headers.Add("X-User-ID", userId);

        httpRequest.Content = JsonContent.Create(request);

        var response = await _dbApiClient.SendAsync(
            httpRequest,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        var message = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        return message ?? new MessageResponseDto();
    }
}