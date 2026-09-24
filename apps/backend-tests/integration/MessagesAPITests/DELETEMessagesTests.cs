using System.Net;
using System.Net.Http.Json;

public class DELETEMessagesTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public DELETEMessagesTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task DeleteMessage_ReturnsNoContent_WhenUserIsSender()
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
        var messageIdToDelete = 1; // set in TestWebApplicationFactory.cs

        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}/messages/{messageIdToDelete}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMessage_ReturnsNotFound_WhenMessageDoesNotExist()
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
        var nonExistentMessageId = 999; // Non-existent message ID

        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}/messages/{nonExistentMessageId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMessage_ReturnsNotFound_WhenUserIsNotSender()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Add nouser to the group chat
        var groupChatId = 1; // set in TestWebApplicationFactory.cs

        await _client.PostAsJsonAsync($"/api/groupchats/{groupChatId}/members", new { UserId = _factory.NoGroupChatsUserId });

        // Logout testuser
        await _client.PostAsJsonAsync("/api/auth/logout", new { });

        // Login as nouser
        var newUserLoginRequest = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", newUserLoginRequest);

        // Check the Messages of the existing group chat
        var messageIdToDelete = 1; // Message sent by another user

        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}/messages/{messageIdToDelete}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMessage_ReturnsUnauthorized_WhenUserNotLoggedIn()
    {
        // Check the Messages of the existing group chat
        var groupChatId = 1; // set in TestWebApplicationFactory.cs
        var messageIdToDelete = 1; // set in TestWebApplicationFactory.cs

        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}/messages/{messageIdToDelete}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}