using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"AuthTests-{Guid.NewGuid()}";
    private readonly Mock<IGroupChatClient> _groupChatClient = new();
    private readonly Dictionary<string, List<GroupChatResponseDto>> _groupChatsByUser = new();
    private int _nextGroupChatId = 2;

    public string TestUserId { get; private set; } = string.Empty;
    public string NoGroupChatsUserId { get; private set; } = string.Empty;

    // Members Mock
    private readonly Mock<IMembersClient> _membersClient = new();
    private readonly Dictionary<int, List<MemberResponseDto>> _membersByGroupChat = new();

    // Messages Mock
    private readonly Mock<IMessagesClient> _messagesClient = new();
    private readonly Dictionary<int, List<MessageResponseDto>> _messagesByGroupChat = new();
    public void ResetGroupChatState()
    {
        _nextGroupChatId = 2;

        _membersByGroupChat.Clear();

        foreach (var groupChats in _groupChatsByUser.Values)
        {
            groupChats.Clear();
        }

        _groupChatsByUser[TestUserId].Add(new GroupChatResponseDto
        {
            Id = 1,
            Name = "Test Group Chat",
            CreatedAt = DateTime.UtcNow
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the application's real database registration
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.RemoveAll<IGroupChatClient>();
            services.RemoveAll<IMembersClient>();
            services.AddScoped(_ => _groupChatClient.Object);
            services.AddScoped(_ => _membersClient.Object);
            services.Configure<IdentityOptions>(options =>
                options.SignIn.RequireConfirmedAccount = false);

            // Add test database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var testUser = SeedUser(userManager, "testuser");
            var noGroupChatsUser = SeedUser(userManager, "nouser");
            TestUserId = testUser.Id;
            NoGroupChatsUserId = noGroupChatsUser.Id;

            var existingGroupChat = new GroupChatResponseDto
            {
                Id = 1,
                Name = "Test Group Chat",
                CreatedAt = DateTime.UtcNow
            };
            _groupChatsByUser[testUser.Id] = new List<GroupChatResponseDto> { existingGroupChat };
            _groupChatsByUser[noGroupChatsUser.Id] = new List<GroupChatResponseDto>();

            _groupChatClient
                .Setup(client => client.GetGroupChatsForUserAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string userId, CancellationToken _) =>
                    _groupChatsByUser.TryGetValue(userId, out var groupChats)
                        ? groupChats.AsEnumerable()
                        : Enumerable.Empty<GroupChatResponseDto>());

            _groupChatClient
                .Setup(client => client.CreateGroupChatAsync(
                    It.IsAny<string>(),
                    It.IsAny<CreateGroupChatRequestDto>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((string _, CreateGroupChatRequestDto request, CancellationToken _) =>
                    new GroupChatResponseDto
                    {
                        Id = _nextGroupChatId++,
                        Name = request.Name,
                        CreatedAt = DateTime.UtcNow
                    });

            _groupChatClient
                .Setup(client => client.AddMembersToGroupChatAsync(
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<int, IEnumerable<string>, CancellationToken>((groupChatId, userIds, _) =>
                {
                    var groupChat = new GroupChatResponseDto
                    {
                        Id = groupChatId,
                        Name = "Another Group Chat",
                        CreatedAt = DateTime.UtcNow
                    };

                    foreach (var userId in userIds)
                    {
                        if (!_groupChatsByUser.TryGetValue(userId, out var groupChats))
                        {
                            groupChats = new List<GroupChatResponseDto>();
                            _groupChatsByUser[userId] = groupChats;
                        }

                        if (groupChats.All(existing => existing.Id != groupChatId))
                        {
                            groupChats.Add(groupChat);
                        }

                        if (!_membersByGroupChat.TryGetValue(groupChatId, out var members))
                        {
                            members = new List<MemberResponseDto>();
                            _membersByGroupChat[groupChatId] = members;
                        }

                        if (members.All(member => member.UserId != userId))
                        {
                            members.Add(new MemberResponseDto
                            {
                                GroupChatId = groupChatId,
                                UserId = userId,
                                JoinedAt = DateTime.UtcNow,
                                LastActiveAt = DateTime.UtcNow
                            });
                        }
                    }
                })
                .Returns(Task.CompletedTask);

            _groupChatClient
                .Setup(client => client.DeleteGroupChatAsync(
                    It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, CancellationToken _) =>
                {
                    var wasDeleted = false;

                    foreach (var groupChats in _groupChatsByUser.Values)
                    {
                        wasDeleted |= groupChats.RemoveAll(groupChat => groupChat.Id == groupChatId) > 0;
                    }

                    return wasDeleted;
                });

            _membersClient
                .Setup(client => client.GetMembersOfGroupChatAsync(
                    It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, CancellationToken _) =>
                    _membersByGroupChat.TryGetValue(groupChatId, out var members)
                        ? members.AsEnumerable()
                        : Enumerable.Empty<MemberResponseDto>());

            _membersClient
                .Setup(client => client.AddMembersToGroupChatAsync(
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, IEnumerable<string> userIds, CancellationToken _) =>
                {
                    if (!_membersByGroupChat.TryGetValue(groupChatId, out var members))
                    {
                        members = new List<MemberResponseDto>();
                        _membersByGroupChat[groupChatId] = members;
                    }

                    var addedMembers = new List<MemberResponseDto>();
                    foreach (var userId in userIds)
                    {
                        if (members.Any(member => member.UserId == userId))
                        {
                            continue; // Skip if the user is already a member
                        }

                        if (members.All(member => member.UserId != userId))
                        {
                            var addedMember = new MemberResponseDto
                            {
                                GroupChatId = groupChatId,
                                UserId = userId,
                                JoinedAt = DateTime.UtcNow,
                                LastActiveAt = DateTime.UtcNow
                            };
                            members.Add(addedMember);
                            addedMembers.Add(addedMember);
                        }
                    }

                    return addedMembers;
                });

            _membersClient
                .Setup(client => client.RemoveMemberFromGroupChatAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, string userId, CancellationToken _) =>
                {
                    if (_membersByGroupChat.TryGetValue(groupChatId, out var members))
                    {
                        var member = members.FirstOrDefault(m => m.UserId == userId);
                        if (member != null)
                        {
                            members.Remove(member);
                            return true;
                        }
                    }
                    return false;
                });
        });
    }

    private static ApplicationUser SeedUser(
        UserManager<ApplicationUser> userManager,
        string username)
    {
        var existingUser = userManager.FindByNameAsync(username)
            .GetAwaiter()
            .GetResult();

        if (existingUser is not null)
        {
            return existingUser;
        }

        var user = new ApplicationUser
        {
            UserName = username,
            Email = $"{username}@example.com",
            EmailConfirmed = true
        };
        var seedResult = userManager.CreateAsync(user, "TestPassword1!")
            .GetAwaiter()
            .GetResult();

        if (!seedResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", seedResult.Errors.Select(error => error.Description)));
        }

        return user;
    }
}