using System.Net.Http.Json;

namespace backend.Tests;

public class GroupChatsAPITests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GroupChatsAPITests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCurrentUserGroupChats_ReturnsSuccess()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode, await loginResponse.Content.ReadAsStringAsync());

        // Act
        var response = await _client.GetAsync("/api/groupchats");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<GroupChatResponseDto>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCurrentUserGroupChats_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("/api/groupchats");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUserGroupChats_ReturnsNotFound_WhenNoGroupChats()
    {
        // Login with a user that has no group chats
        var loginRequest = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode, await loginResponse.Content.ReadAsStringAsync());

        // Act
        var response = await _client.GetAsync("/api/groupchats");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostNewGroupChat_ReturnsCreated()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode, await loginResponse.Content.ReadAsStringAsync());

        // Act
        var newGroupChatRequest = new CreateGroupChatRequestDto
        {
            Name = "New Group Chat",
            UserIds = new List<string> { "user1", "user2" }
        };
        var response = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

}