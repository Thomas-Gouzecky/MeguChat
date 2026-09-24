using System.Net.Http.Json;

public class GroupChatCreationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GroupChatCreationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
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
        response.EnsureSuccessStatusCode();
        var groupChatResponse = await response.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        Assert.NotNull(groupChatResponse);
        Assert.Equal(newGroupChatRequest.Name, groupChatResponse.Name);
        Assert.True(groupChatResponse.Id > 0);
    }

    [Fact]
    public async Task PostNewGroupChat_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var newGroupChatRequest = new CreateGroupChatRequestDto
        {
            Name = "New Group Chat",
            UserIds = new List<string> { "user1", "user2" }
        };
        var response = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostNewGroupChat_ReturnsBadRequest_WhenNameIsMissing()
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
            Name = "", // Missing name
            UserIds = new List<string> { "user1", "user2" }
        };
        var response = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}