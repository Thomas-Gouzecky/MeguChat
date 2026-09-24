public class MessagesClient : DatabaseClient, IMessagesClient
{
    public MessagesClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }
}