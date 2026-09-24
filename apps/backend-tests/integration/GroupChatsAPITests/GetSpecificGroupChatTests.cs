using System.Net;
using System.Net.Http.Json;

public class GetSpecificGroupChatTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public GetSpecificGroupChatTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }


    [Fact]
    public async Task GetSpecificGroupChat_ReturnsGroupChat_WhenGroupChatExists()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Create a new group chat
        var newGroupChatRequest = new CreateGroupChatRequestDto
        {
            Name = "Test Group Chat",
            UserIds = new List<string> { "user1", "user2" }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdGroupChat = await createResponse.Content.ReadFromJsonAsync<GroupChatResponseDto>();

        var groupChatId = createdGroupChat?.Id;

        // Act
        var response = await _client.GetAsync($"/api/groupchats/{groupChatId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var groupChat = await response.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        Assert.NotNull(groupChat);
        Assert.Equal(groupChatId, groupChat.Id);
    }

    [Fact]
    public async Task GetSpecificGroupChat_ReturnsNotFound_WhenGroupChatDoesNotExist()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Arrange
        var nonExistentGroupChatId = 9999;

        // Act
        var response = await _client.GetAsync($"/api/groupchats/{nonExistentGroupChatId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSpecificGroupChat_ReturnsNotFound_WhenUserIsNotMemberOfGroupChat()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Create a new group chat with different users
        var newGroupChatRequest = new CreateGroupChatRequestDto
        {
            Name = "Another Group Chat",
            UserIds = new List<string> { "user3", "user4" }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdGroupChat = await createResponse.Content.ReadFromJsonAsync<GroupChatResponseDto>();

        var groupChatId = createdGroupChat?.Id;

        Assert.NotNull(createdGroupChat);
        Assert.Equal(groupChatId, createdGroupChat.Id);

        // Logout
        await _client.PostAsync("/api/auth/logout", null);

        // Login as a different user who is not a member of the group chat
        var loginRequest2 = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest2);

        // Act
        var response = await _client.GetAsync($"/api/groupchats/{groupChatId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSpecificGroupChat_ReturnsGroupChat_WhenAddedMemberOfGroupChat()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Create a new group chat with different users
        var newGroupChatRequest = new CreateGroupChatRequestDto
        {
            Name = "Another Group Chat",
            UserIds = new List<string> { "user3", "user4", _factory.NoGroupChatsUserId }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdGroupChat = await createResponse.Content.ReadFromJsonAsync<GroupChatResponseDto>();

        var groupChatId = createdGroupChat?.Id;

        Assert.NotNull(createdGroupChat);
        Assert.Equal(groupChatId, createdGroupChat.Id);

        // Logout
        await _client.PostAsync("/api/auth/logout", null);

        // Login as a different user who is a member of the group chat
        var loginRequest2 = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest2);

        // Act
        var response = await _client.GetAsync($"/api/groupchats/{groupChatId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var groupChat = await response.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        Assert.NotNull(groupChat);
        Assert.Equal(groupChatId, groupChat.Id);
    }
}