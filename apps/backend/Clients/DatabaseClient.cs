using System.Text.Json;

public abstract class DatabaseClient
{
    protected readonly HttpClient _dbApiClient;

    protected DatabaseClient(IHttpClientFactory httpClientFactory)
    {
        _dbApiClient = httpClientFactory.CreateClient("dbApi");
    }

    protected static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            DatabaseErrorDto body;

            try
            {
                body = await response.Content.ReadFromJsonAsync<DatabaseErrorDto>() ?? new DatabaseErrorDto();
            }
            catch (JsonException)
            {
                body = new DatabaseErrorDto();
            }

            throw new DbApiException(response.StatusCode, body);
        }
    }
}