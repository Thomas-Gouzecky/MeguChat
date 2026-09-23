using System.Net;
using System.Net.Http.Json;

public class DELETESpecificGroupChatTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DELETESpecificGroupChatTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeleteSpecificGroupChat_ReturnsNoContent_WhenGroupChatExists()
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
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }

    [Fact]
    public async Task DeleteSpecificGroupChat_ReturnsNotFound_WhenGroupChatDoesNotExist()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/9999"); // Assuming 9999 is a non-existent group chat ID

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSpecificGroupChat_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/1"); // Assuming 1 is a valid group chat ID

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSpecificGroupChat_ReturnsNotFound_WhenUserIsNotMember()
    {
        // Login with a user that is not a member of the group chat
        var loginRequest = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/1"); // Assuming 1 is a valid group chat ID

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSpecificGroupChat_ReturnsBadRequest_WhenGroupChatIdIsInvalid()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/invalid-id"); // Invalid group chat ID

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSpecificGroupChat_ReturnsNotFound_WhenGroupChatAlreadyDeleted()
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

        // Delete the group chat
        var deleteResponse = await _client.DeleteAsync($"/api/groupchats/{groupChatId}");
        deleteResponse.EnsureSuccessStatusCode();

        // Act - Try to delete again
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSpecificGroupChat_ReturnsNotFound_WhenDifferentMemberViewsGroupChatAfterDeletion()
    {
        // Login with a user that is a member of the group chat
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
            UserIds = new List<string> { "user1", "user2", "nouser" }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdGroupChat = await createResponse.Content.ReadFromJsonAsync<GroupChatResponseDto>();

        var groupChatId = createdGroupChat?.Id;

        // Delete the group chat
        var deleteResponse = await _client.DeleteAsync($"/api/groupchats/{groupChatId}");
        deleteResponse.EnsureSuccessStatusCode();

        // Login with a different member of the group chat
        var loginRequest2 = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest2);

        // Act - Try to view the deleted group chat
        var response = await _client.GetAsync($"/api/groupchats/{groupChatId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}