using System.Net;
using System.Net.Http.Json;

public class DELETEMembersTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public DELETEMembersTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task RemoveMember_RemovesMemberFromGroupChat_WhenMemberExists()
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
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}/members/user1");

        // Assert
        response.EnsureSuccessStatusCode();

        // Validate that the member was removed by fetching the members of the group chat
        var membersResponse = await _client.GetAsync($"/api/groupchats/{groupChatId}/members");
        membersResponse.EnsureSuccessStatusCode();
        var members = await membersResponse.Content.ReadFromJsonAsync<IEnumerable<MemberResponseDto>>();
        Assert.NotNull(members);
        Assert.DoesNotContain(members, m => m.UserId == "user1");
    }

    [Fact]
    public async Task RemoveMember_ReturnsNoContent_WhenMemberExists()
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
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}/members/user1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveMember_ReturnsNotFound_WhenMemberDoesNotExist()
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
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}/members/nonexistentuser");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveMember_ReturnsUnauthorized_WhenUserNotLoggedIn()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/1/members/user1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}