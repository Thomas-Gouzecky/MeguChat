using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

public class MessagesClient : DatabaseClient, IMessagesClient
{
    public MessagesClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, string userId, int limit, DateTime? before_created_at, CancellationToken cancellationToken = default)
    {
        var url = QueryHelpers.AddQueryString(
            $"/api/groupchats/{groupChatId}/messages",
            new Dictionary<string, string?>
            {
                { "limit", limit.ToString() },
                { "before_created_at", before_created_at?.ToString("o") }
            }
        );

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            url
        );

        httpRequest.Headers.Add("X-User-ID", userId);

        var response = await _dbApiClient.SendAsync(
            httpRequest,
            cancellationToken
        );
        await EnsureSuccessAsync(response, cancellationToken);


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
        await EnsureSuccessAsync(response, cancellationToken);

        var message = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        return message ?? throw new InvalidOperationException("Failed to send message.");
    }

    public async Task<MessageResponseDto> DeleteMessageFromGroupChatAsync(int groupChatId, int messageId, string userId, CancellationToken cancellationToken = default)
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
        await EnsureSuccessAsync(response, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var deletedMessage = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        return deletedMessage ?? throw new InvalidOperationException("Failed to delete message.");
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
        await EnsureSuccessAsync(response, cancellationToken);

        var message = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        return message ?? throw new InvalidOperationException("Failed to update message.");
    }
}