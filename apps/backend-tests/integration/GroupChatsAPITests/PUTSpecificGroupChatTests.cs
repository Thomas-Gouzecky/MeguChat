using System.Net;
using System.Net.Http.Json;

public class PUTSpecificGroupChatTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public PUTSpecificGroupChatTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task UpdateSpecificGroupChat_ReturnsUpdatedGroupChat_WhenGroupChatExists()
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
        var updateGroupChatRequest = new UpdateGroupChatRequestDto
        {
            Name = "Updated Group Chat"
        };
        var response = await _client.PutAsJsonAsync($"/api/groupchats/{groupChatId}", updateGroupChatRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedGroupChat = await response.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        Assert.NotNull(updatedGroupChat);
        Assert.Equal(groupChatId, updatedGroupChat.Id);
        Assert.Equal(updateGroupChatRequest.Name, updatedGroupChat.Name);
    }

    [Fact]
    public async Task UpdateSpecificGroupChat_ReturnsNotFound_WhenGroupChatDoesNotExist()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Act
        var updateGroupChatRequest = new UpdateGroupChatRequestDto
        {
            Name = "Updated Group Chat"
        };
        var response = await _client.PutAsJsonAsync($"/api/groupchats/9999", updateGroupChatRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSpecificGroupChat_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var updateGroupChatRequest = new UpdateGroupChatRequestDto
        {
            Name = "Updated Group Chat"
        };
        var response = await _client.PutAsJsonAsync($"/api/groupchats/1", updateGroupChatRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSpecificGroupChat_ReturnsBadRequest_WhenNameIsMissing()
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
        var updateGroupChatRequest = new UpdateGroupChatRequestDto
        {
            Name = "" // Missing name
        };
        var response = await _client.PutAsJsonAsync($"/api/groupchats/{groupChatId}", updateGroupChatRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSpecificGroupChat_ReturnsNotFound_WhenUserIsNotMemberOfGroupChat()
    {
        // Login with a user that is not a member of the group chat
        var loginRequest = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Create a new group chat with a different user
        var newGroupChatRequest = new CreateGroupChatRequestDto
        {
            Name = "Test Group Chat",
            UserIds = new List<string> { "user1", "user2" }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdGroupChat = await createResponse.Content.ReadFromJsonAsync<GroupChatResponseDto>();
        var groupChatId = createdGroupChat?.Id;

        // Logout
        await _client.PostAsync("/api/auth/logout", null);

        // Login as a different user who is not a member of the group chat
        var loginRequest2 = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest2);

        // Act
        var updateGroupChatRequest = new UpdateGroupChatRequestDto
        {
            Name = "Updated Group Chat"
        };
        var response = await _client.PutAsJsonAsync($"/api/groupchats/{groupChatId}", updateGroupChatRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}