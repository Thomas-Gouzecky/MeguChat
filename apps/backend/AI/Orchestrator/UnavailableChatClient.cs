using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

public sealed class UnavailableChatClient : IChatClient
{
    public ChatClientMetadata Metadata => new("AI unavailable");

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new AIUnavailableException();
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        throw new AIUnavailableException();
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return null;
    }

    public void Dispose()
    {
    }
}
