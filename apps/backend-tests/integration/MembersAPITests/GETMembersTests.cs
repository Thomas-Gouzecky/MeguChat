using System.Net;
using System.Net.Http.Json;

public class GETMembersTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public GETMembersTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task GetMembers_ReturnsMembers_WhenGroupChatExists()
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
        var response = await _client.GetAsync($"/api/groupchats/{groupChatId}/members");

        // Assert
        response.EnsureSuccessStatusCode();
        var members = await response.Content.ReadFromJsonAsync<List<MemberResponseDto>>();
        Assert.NotNull(members);

        // Includes the creator of the group chat and the two users added
        Assert.Equal(3, members.Count);
    }

    [Fact]
    public async Task GetMembers_ReturnsNotFound_WhenGroupChatDoesNotExist()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Act
        var response = await _client.GetAsync($"/api/groupchats/9999/members");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMembers_ReturnsUnauthorized_WhenUserNotLoggedIn()
    {
        // Act
        var response = await _client.GetAsync($"/api/groupchats/1/members");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMembers_ReturnsNotFound_WhenUserIsNotMemberOfGroupChat()
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

        // Login as a different user who is not a member of the group chat
        await _client.PostAsJsonAsync("/api/auth/logout", new { });

        var loginRequest2 = new
        {
            username = "nouser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest2);

        // Act
        var response = await _client.GetAsync($"/api/groupchats/{groupChatId}/members");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMembers_ReturnsBadRequest_WhenGroupChatIdIsInvalid()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Act
        var response = await _client.GetAsync($"/api/groupchats/invalid/members");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}