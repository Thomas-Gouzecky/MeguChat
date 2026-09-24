using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

public class POSTMembersTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public POSTMembersTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task AddMemberToGroupChat_ShouldReturnSuccess()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Arrange
        var groupChatId = 1; // Replace with a valid group chat ID
        var requestDto = new AddMemberRequestDto
        {
            UserId = new List<string> { "nouser" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var addedMembers = JsonConvert.DeserializeObject<IEnumerable<MemberResponseDto>>(responseString);

        Assert.NotNull(addedMembers);
        Assert.Equal(requestDto.UserId.Count(), addedMembers.Count());
    }

    [Fact]
    public async Task AddMemberToGroupChat_ShouldReturnNotFound_WhenGroupChatDoesNotExist()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Arrange
        var groupChatId = 9999; // Non-existent group chat ID
        var requestDto = new AddMemberRequestDto
        {
            UserId = new List<string> { "nouser" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddMemberToGroupChat_ShouldReturnUnauthorized_WhenUserNotLoggedIn()
    {
        // Arrange
        var groupChatId = 1; // Replace with a valid group chat ID
        var requestDto = new AddMemberRequestDto
        {
            UserId = new List<string> { "nouser" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddMemberToGroupChat_ShouldReturnNotFound_WhenUserIsNotMemberOfGroupChat()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Arrange
        var groupChatId = 2;
        var requestDto = new AddMemberRequestDto
        {
            UserId = new List<string> { "nouser" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddMemberToGroupChat_ShouldReturnBadRequest_WhenUserIdIsMissing()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Arrange
        var groupChatId = 1; // Replace with a valid group chat ID
        var requestDto = new AddMemberRequestDto
        {
            UserId = new List<string>() // Empty list to simulate missing user ID
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddMemberToGroupChat_ShouldAddOtherMembers_WhenUserIsAlreadyMember()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Arrange
        var groupChatId = 1; // Replace with a valid group chat ID
        var requestDto = new AddMemberRequestDto
        {
            UserId = new List<string> { "nouser" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var addedMembers = JsonConvert.DeserializeObject<IEnumerable<MemberResponseDto>>(responseString);

        Assert.NotNull(addedMembers);
        Assert.Equal(requestDto.UserId.Count(), addedMembers.Count());

        var requestDto2 = new AddMemberRequestDto
        {
            UserId = new List<string> { "nouser", "user3" }
        };

        content = new StringContent(JsonConvert.SerializeObject(requestDto2), Encoding.UTF8, "application/json");

        // Act
        response = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);

        // Assert
        response.EnsureSuccessStatusCode();
        responseString = await response.Content.ReadAsStringAsync();
        addedMembers = JsonConvert.DeserializeObject<IEnumerable<MemberResponseDto>>(responseString);

        Assert.NotNull(addedMembers);
        Assert.Equal(requestDto.UserId.Count(), addedMembers.Count());
    }

    [Fact]
    public async Task AddMemberToGroupChat_ShouldAddMembersOnce_WhenAddingSameMemberMultipleTimes()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Arrange
        var groupChatId = 1; // Replace with a valid group chat ID
        var requestDto = new AddMemberRequestDto
        {
            UserId = new List<string> { "nouser" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

        // Act
        var response1 = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);
        response1.EnsureSuccessStatusCode();

        var response2 = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);
        response2.EnsureSuccessStatusCode();

        // Assert
        var responseString1 = await response1.Content.ReadAsStringAsync();
        var addedMembers1 = JsonConvert.DeserializeObject<IEnumerable<MemberResponseDto>>(responseString1);

        var responseString2 = await response2.Content.ReadAsStringAsync();
        var addedMembers2 = JsonConvert.DeserializeObject<IEnumerable<MemberResponseDto>>(responseString2);

        // Get the actual membership state
        var members = await _client.GetFromJsonAsync<List<MemberResponseDto>>(
            $"/api/groupchats/{groupChatId}/members"
        );

        Assert.NotNull(addedMembers1);
        Assert.NotNull(addedMembers2);
        Assert.NotNull(members);
        Assert.NotEmpty(members);
        Assert.Single(members, member => member.UserId == "nouser");
    }
}