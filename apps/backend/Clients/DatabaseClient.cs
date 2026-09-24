public abstract class DatabaseClient
{
    protected readonly HttpClient _dbApiClient;

    protected DatabaseClient(IHttpClientFactory httpClientFactory)
    {
        _dbApiClient = httpClientFactory.CreateClient("dbApi");
    }
}