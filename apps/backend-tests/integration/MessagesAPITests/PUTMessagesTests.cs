using System.Net;
using System.Net.Http.Json;

public class PUTMessagesTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public PUTMessagesTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task UpdateMessage_ReturnsModifiedMessage_WhenUserIsSender()
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
        var messageIdToUpdate = 1; // set in TestWebApplicationFactory.cs

        // Act
        var updateRequest = new MessageUpdateRequestDto
        {
            Message = "Updated message content."
        };
        var response = await _client.PutAsJsonAsync($"/api/groupchats/{groupChatId}/messages/{messageIdToUpdate}", updateRequest);

        var updatedMessage = await response.Content.ReadFromJsonAsync<MessageResponseDto>();

        // Assert
        Assert.NotNull(updatedMessage);
        Assert.Equal(messageIdToUpdate, updatedMessage.Id);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Updated message content.", updatedMessage.Message);
    }

    [Fact]
    public async Task UpdateMessage_ReturnsNotFound_WhenUserIsNotInGroupChat()
    {
        // Login
        var loginRequest = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Check the Messages of the existing group chat
        var groupChatId = 1; // set in TestWebApplicationFactory.cs
        var messageIdToUpdate = 1; // set in TestWebApplicationFactory.cs

        // Act
        var updateRequest = new MessageUpdateRequestDto
        {
            Message = "Updated message content."
        };
        var response = await _client.PutAsJsonAsync($"/api/groupchats/{groupChatId}/messages/{messageIdToUpdate}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}