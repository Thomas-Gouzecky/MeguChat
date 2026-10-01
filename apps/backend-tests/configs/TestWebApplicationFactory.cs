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
        _messagesByGroupChat.Clear();

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

        AddMember(1, TestUserId);

        AddMessage(1, "Hello from testuser!", TestUserId);
        AddMessage(1, "Hello Again!", TestUserId);
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
            services.RemoveAll<IMessagesClient>();
            services.AddScoped(_ => _groupChatClient.Object);
            services.AddScoped(_ => _membersClient.Object);
            services.AddScoped(_ => _messagesClient.Object);
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
            AddMember(existingGroupChat.Id, testUser.Id);

            AddMessage(existingGroupChat.Id, "Hello from testuser!", testUser.Id);
            AddMessage(existingGroupChat.Id, "Hello Again!", testUser.Id);

            _groupChatClient
                .Setup(client => client.GetGroupChatsForUserAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string userId, CancellationToken _) =>
                    _groupChatsByUser.TryGetValue(userId, out var groupChats)
                        ? groupChats.AsEnumerable()
                        : Enumerable.Empty<GroupChatResponseDto>());

            _groupChatClient
                .Setup(client => client.GetGroupChatByIdAsync(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((int groupChatId, string userId, CancellationToken _) =>
                    Task.FromResult<GroupChatResponseDto>(
                        _groupChatsByUser.TryGetValue(userId, out var groupChats)
                            ? groupChats.FirstOrDefault(groupChat => groupChat.Id == groupChatId)!
                            : null!));

            _groupChatClient
                .Setup(client => client.CreateGroupChatAsync(
                    It.IsAny<string>(),
                    It.IsAny<CreateGroupChatRequestDto>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((string userId, CreateGroupChatRequestDto request, CancellationToken _) =>
                    {
                        var groupChat = new GroupChatResponseDto
                        {
                            Id = _nextGroupChatId++,
                            Name = request.Name,
                            CreatedAt = DateTime.UtcNow
                        };

                        if (!_groupChatsByUser.TryGetValue(userId, out var groupChats))
                        {
                            groupChats = new List<GroupChatResponseDto>();
                            _groupChatsByUser[userId] = groupChats;
                        }

                        groupChats.Add(groupChat);
                        AddMember(groupChat.Id, userId);
                        foreach (var requestedUserId in request.UserIds ?? [])
                        {
                            AddMember(groupChat.Id, requestedUserId);
                        }

                        return groupChat;
                    });

            _groupChatClient
                .Setup(client => client.UpdateGroupChatAsync(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<UpdateGroupChatRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, string userId, UpdateGroupChatRequestDto request, CancellationToken _) =>
                {
                    var groupChat = FindUserGroupChat(groupChatId, userId);
                    if (groupChat is null)
                    {
                        throw new NotFoundException("Group chat not found.");
                    }

                    groupChat.Name = request.Name;
                    return groupChat;
                });


            _groupChatClient
                .Setup(client => client.DeleteGroupChatAsync(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, string currentUserId, CancellationToken _) =>
                {
                    var deletedGroupChat = FindUserGroupChat(groupChatId, currentUserId);
                    if (deletedGroupChat is null)
                    {
                        throw new NotFoundException("Group chat not found.");
                    }

                    foreach (var groupChats in _groupChatsByUser.Values)
                    {
                        groupChats.RemoveAll(groupChat => groupChat.Id == groupChatId);
                    }

                    _membersByGroupChat.Remove(groupChatId);
                    _messagesByGroupChat.Remove(groupChatId);
                    return deletedGroupChat;
                });

            _membersClient
                .Setup(client => client.GetMembersOfGroupChatAsync(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, string currentUserId, CancellationToken _) =>
                {
                    EnsureMember(groupChatId, currentUserId);
                    return _membersByGroupChat[groupChatId].AsEnumerable();
                });

            _membersClient
                .Setup(client => client.AddMembersToGroupChatAsync(
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, IEnumerable<string> userIds, string currentUserId, CancellationToken _) =>
                {
                    EnsureMember(groupChatId, currentUserId);

                    if (!_membersByGroupChat.TryGetValue(groupChatId, out var members))
                    {
                        members = new List<MemberResponseDto>();
                        _membersByGroupChat[groupChatId] = members;
                    }

                    var addedMembers = new List<MemberResponseDto>();
                    foreach (var userId in userIds)
                    {
                        if (!_groupChatsByUser.TryGetValue(userId, out var groupChats))
                        {
                            groupChats = new List<GroupChatResponseDto>();
                            _groupChatsByUser[userId] = groupChats;
                        }

                        if (groupChats.All(groupChat => groupChat.Id != groupChatId))
                        {
                            groupChats.Add(new GroupChatResponseDto
                            {
                                Id = groupChatId,
                                Name = "Test Group Chat",
                                CreatedAt = DateTime.UtcNow
                            });
                        }

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
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, string userId, string currentUserId, CancellationToken _) =>
                {
                    EnsureMember(groupChatId, currentUserId);

                    if (_membersByGroupChat.TryGetValue(groupChatId, out var members))
                    {
                        var member = members.FirstOrDefault(m => m.UserId == userId);
                        if (member != null)
                        {
                            members.Remove(member);
                            return member;
                        }
                    }
                    return new MemberResponseDto();
                });

            _messagesClient
                .Setup(client => client.GetMessagesOfGroupChatAsync(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, string userId, CancellationToken _) =>
                {
                    EnsureMember(groupChatId, userId);
                    return _messagesByGroupChat.TryGetValue(groupChatId, out var messages)
                        ? messages.AsEnumerable()
                        : Enumerable.Empty<MessageResponseDto>();
                });

            _messagesClient
                .Setup(client => client.SendMessageToGroupChatAsync(
                    It.IsAny<int>(),
                    It.IsAny<MessageCreationRequestDto>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, MessageCreationRequestDto request, string userId, CancellationToken _) =>
                {
                    EnsureMember(groupChatId, userId);
                    return AddMessage(groupChatId, request.Content, userId);
                });

            _messagesClient
                .Setup(client => client.DeleteMessageFromGroupChatAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, int messageId, string userId, CancellationToken _) =>
                {
                    EnsureMember(groupChatId, userId);

                    if (_messagesByGroupChat.TryGetValue(groupChatId, out var messages))
                    {
                        var message = messages.FirstOrDefault(m => m.Id == messageId && m.UserId == userId);

                        if (message != null)
                        {
                            messages.Remove(message);
                            return message;
                        }
                    }
                    throw new NotFoundException("Message not found.");
                });

            _messagesClient
                .Setup(client => client.UpdateMessageInGroupChatAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<MessageUpdateRequestDto>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((int groupChatId, int messageId, MessageUpdateRequestDto request, string userId, CancellationToken _) =>
                {
                    EnsureMember(groupChatId, userId);

                    if (!_messagesByGroupChat.TryGetValue(groupChatId, out var messages))
                    {
                        throw new NotFoundException("Group chat not found.");
                    }

                    var message = messages.FirstOrDefault(m => m.Id == messageId);

                    if (message == null)
                    {
                        throw new NotFoundException("Message not found.");
                    }

                    if (message.UserId != userId)
                    {
                        throw new UnauthorizedAccessException(
                            "You are not authorized to update this message."
                        );
                    }

                    message.Content = request.Content;
                    message.ModifiedAt = DateTime.UtcNow;

                    return message;
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

    private MessageResponseDto AddMessage(int groupChatId, string content, string userId)
    {
        if (!_messagesByGroupChat.TryGetValue(groupChatId, out var messages))
        {
            messages = new List<MessageResponseDto>();
            _messagesByGroupChat[groupChatId] = messages;
        }

        var message = new MessageResponseDto
        {
            Id = messages.Count + 1,
            Content = content,
            UserId = userId,
            GroupChatId = groupChatId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        messages.Add(message);
        return message;
    }

    private void AddMember(int groupChatId, string userId)
    {
        var groupChat = _groupChatsByUser.Values
            .SelectMany(groupChats => groupChats)
            .FirstOrDefault(candidate => candidate.Id == groupChatId);

        if (groupChat is not null &&
            (!_groupChatsByUser.TryGetValue(userId, out var userGroupChats) ||
             userGroupChats.All(candidate => candidate.Id != groupChatId)))
        {
            userGroupChats ??= new List<GroupChatResponseDto>();
            userGroupChats.Add(groupChat);
            _groupChatsByUser[userId] = userGroupChats;
        }

        if (!_membersByGroupChat.TryGetValue(groupChatId, out var members))
        {
            members = new List<MemberResponseDto>();
            _membersByGroupChat[groupChatId] = members;
        }

        if (members.Any(member => member.UserId == userId))
        {
            return;
        }

        members.Add(new MemberResponseDto
        {
            Id = members.Count + 1,
            GroupChatId = groupChatId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            LastActiveAt = DateTime.UtcNow
        });
    }

    private GroupChatResponseDto? FindUserGroupChat(int groupChatId, string userId)
    {
        return _groupChatsByUser.TryGetValue(userId, out var groupChats)
            ? groupChats.FirstOrDefault(groupChat => groupChat.Id == groupChatId)
            : null;
    }

    private void EnsureMember(int groupChatId, string userId)
    {
        if (FindUserGroupChat(groupChatId, userId) is null)
        {
            throw new NotFoundException("Group chat not found for the current user.");
        }
    }
}