using System.Net;
using System.Net.Http.Json;

public class POSTMessagesTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public POSTMessagesTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task PostMessage_ReturnsCreatedMessage_WhenUserIsMember()
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
        var messageRequest = new MessageCreationRequestDto
        {
            Message = "This is a test message."
        };
        var response = await _client.PostAsJsonAsync($"/api/groupchats/{groupChatId}/messages", messageRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdMessage = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        Assert.NotNull(createdMessage);
        Assert.Equal(messageRequest.Message, createdMessage.Message);
    }
}