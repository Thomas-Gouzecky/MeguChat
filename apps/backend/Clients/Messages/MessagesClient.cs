using System.Net.Http.Json;
using System.Text;

public class MessagesClient : DatabaseClient, IMessagesClient
{
    public MessagesClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, string userId, CancellationToken cancellationToken = default)
    {

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/groupchats/{groupChatId}/messages"
        );

        httpRequest.Headers.Add("X-User-ID", userId);

        var response = await _dbApiClient.SendAsync(
            httpRequest,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();


        var messages = await response.Content.ReadFromJsonAsync<List<MessageResponseDto>>();
        return messages ?? throw new InvalidOperationException("Failed to retrieve messages.");
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
        return message ?? throw new InvalidOperationException("Failed to send message.");
    }

    public async Task<bool> DeleteMessageFromGroupChatAsync(int groupChatId, int messageId, string userId, CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/groupchats/{groupChatId}/messages/{messageId}"
        );

        httpRequest.Headers.Add("X-User-ID", userId);

        var response = await _dbApiClient.SendAsync(
            httpRequest,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<MessageResponseDto> UpdateMessageInGroupChatAsync(int groupChatId, int messageId, MessageUpdateRequestDto request, string userId, CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/groupchats/{groupChatId}/messages/{messageId}"
        );

        httpRequest.Headers.Add("X-User-ID", userId);

        httpRequest.Content = JsonContent.Create(request);

        var response = await _dbApiClient.SendAsync(
            httpRequest,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        var message = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        return message ?? throw new InvalidOperationException("Failed to update message.");
    }
}