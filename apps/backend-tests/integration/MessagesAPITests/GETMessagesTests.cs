using System.Net;
using System.Net.Http.Json;

public class GETMessagesTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public GETMessagesTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task GetMessages_ReturnsMessages_WhenGroupChatExists()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Check the Messages of the existing group chat
        var groupChatId = 1; // set in TestWebApplicationFactory.cs

        // Act
        var response = await _client.GetAsync($"/api/groupchats/{groupChatId}/messages");

        // Assert
        response.EnsureSuccessStatusCode();
        var messages = await response.Content.ReadFromJsonAsync<List<MessageResponseDto>>();
        Assert.NotNull(messages);

        // Includes the messages in the group chat
        Assert.Equal(2, messages.Count);
    }
}