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

    [Fact]
    public async Task GetMessages_ReturnsLatestMessages_WhenLimitIsSpecified()
    {
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var response = await _client.GetAsync("/api/groupchats/1/messages?limit=1");

        response.EnsureSuccessStatusCode();
        var messages = await response.Content.ReadFromJsonAsync<List<MessageResponseDto>>();

        Assert.NotNull(messages);
        var message = Assert.Single(messages);
        Assert.Equal("Hello Again!", message.Content);
    }

    [Fact]
    public async Task GetMessages_ReturnsMessagesBeforeCursor_WhenBeforeCreatedAtIsSpecified()
    {
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var allMessagesResponse = await _client.GetAsync("/api/groupchats/1/messages");
        allMessagesResponse.EnsureSuccessStatusCode();
        var allMessages = await allMessagesResponse.Content.ReadFromJsonAsync<List<MessageResponseDto>>();
        Assert.NotNull(allMessages);

        var cursor = allMessages[1].CreatedAt.ToString("O");
        var response = await _client.GetAsync($"/api/groupchats/1/messages?before_created_at={Uri.EscapeDataString(cursor)}");

        response.EnsureSuccessStatusCode();
        var messages = await response.Content.ReadFromJsonAsync<List<MessageResponseDto>>();

        Assert.NotNull(messages);
        var message = Assert.Single(messages);
        Assert.Equal("Hello from testuser!", message.Content);
    }

    [Fact]
    public async Task GetMessages_ReturnsMessages_WhenNonSenderViewsMessages()
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

        // Add a new user to the group chat
        var addUserRequest = new AddMemberRequestDto
        {
            UserId = new List<string> { _factory.NoGroupChatsUserId }
        };
        var response = await _client.PostAsJsonAsync($"/api/groupchats/{groupChatId}/members", addUserRequest);
        response.EnsureSuccessStatusCode();

        // Login as the new user
        response = await _client.PostAsJsonAsync("/api/auth/logout", new { });
        response.EnsureSuccessStatusCode();

        var newUserLoginRequest = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        response = await _client.PostAsJsonAsync("/api/auth/login", newUserLoginRequest);
        response.EnsureSuccessStatusCode();

        // Act
        response = await _client.GetAsync($"/api/groupchats/{groupChatId}/messages");

        // Assert
        response.EnsureSuccessStatusCode();
        var messages = await response.Content.ReadFromJsonAsync<List<MessageResponseDto>>();
        Assert.NotNull(messages);

        // Includes the messages in the group chat
        Assert.Equal(2, messages.Count);
    }

    [Fact]
    public async Task GetMessages_ReturnsNotFound_WhenGroupChatDoesNotExist()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Act
        var response = await _client.GetAsync($"/api/groupchats/9999/messages");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMessages_ReturnsUnauthorized_WhenUserNotLoggedIn()
    {
        // Act
        var response = await _client.GetAsync($"/api/groupchats/1/messages");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMessages_ReturnsNotFound_WhenUserIsNotMemberOfGroupChat()
    {
        // Login
        var loginRequest = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var groupChatId = 1; // set in TestWebApplicationFactory.cs
        // Act
        var response = await _client.GetAsync($"/api/groupchats/{groupChatId}/messages");
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}