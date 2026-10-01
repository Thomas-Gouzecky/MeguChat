public sealed class TestDataStore
{
    private readonly Dictionary<string, List<GroupChatResponseDto>> _groupChatsByUser = new();
    private readonly Dictionary<int, List<MemberResponseDto>> _membersByGroupChat = new();
    private readonly Dictionary<int, List<MessageResponseDto>> _messagesByGroupChat = new();
    private int _nextGroupChatId;

    public void Reset(string testUserId, string noGroupChatsUserId)
    {
        _groupChatsByUser.Clear();
        _membersByGroupChat.Clear();
        _messagesByGroupChat.Clear();
        _nextGroupChatId = 2;

        _groupChatsByUser[testUserId] = new List<GroupChatResponseDto>();
        _groupChatsByUser[noGroupChatsUserId] = new List<GroupChatResponseDto>();

        var groupChat = new GroupChatResponseDto
        {
            Id = 1,
            Name = "Test Group Chat",
            CreatedAt = DateTime.UtcNow
        };

        _groupChatsByUser[testUserId].Add(groupChat);
        AddMember(1, testUserId);
        AddMessage(1, "Hello from testuser!", testUserId);
        AddMessage(1, "Hello Again!", testUserId);
    }

    public IEnumerable<GroupChatResponseDto> GetGroupChatsForUser(string userId)
    {
        return _groupChatsByUser.TryGetValue(userId, out var groupChats)
            ? groupChats.AsEnumerable()
            : Enumerable.Empty<GroupChatResponseDto>();
    }

    public GroupChatResponseDto? GetGroupChat(int groupChatId, string userId)
    {
        return _groupChatsByUser.TryGetValue(userId, out var groupChats)
            ? groupChats.FirstOrDefault(groupChat => groupChat.Id == groupChatId)
            : null;
    }

    public GroupChatResponseDto CreateGroupChat(string userId, CreateGroupChatRequestDto request)
    {
        var groupChat = new GroupChatResponseDto
        {
            Id = _nextGroupChatId++,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        AddGroupChatForUser(groupChat, userId);
        AddMember(groupChat.Id, userId);

        foreach (var requestedUserId in request.UserIds ?? [])
        {
            AddMember(groupChat.Id, requestedUserId);
        }

        return groupChat;
    }

    public GroupChatResponseDto UpdateGroupChat(
        int groupChatId,
        string userId,
        UpdateGroupChatRequestDto request)
    {
        var groupChat = GetRequiredGroupChat(groupChatId, userId);
        groupChat.Name = request.Name;
        return groupChat;
    }

    public GroupChatResponseDto DeleteGroupChat(int groupChatId, string userId)
    {
        var groupChat = GetRequiredGroupChat(groupChatId, userId);

        foreach (var groupChats in _groupChatsByUser.Values)
        {
            groupChats.RemoveAll(candidate => candidate.Id == groupChatId);
        }

        _membersByGroupChat.Remove(groupChatId);
        _messagesByGroupChat.Remove(groupChatId);
        return groupChat;
    }

    public IEnumerable<MemberResponseDto> GetMembers(int groupChatId, string userId)
    {
        EnsureMember(groupChatId, userId);
        return _membersByGroupChat[groupChatId].AsEnumerable();
    }

    public IEnumerable<MemberResponseDto> AddMembers(
        int groupChatId,
        IEnumerable<string> userIds,
        string currentUserId)
    {
        EnsureMember(groupChatId, currentUserId);

        var addedMembers = new List<MemberResponseDto>();
        foreach (var userId in userIds)
        {
            if (_membersByGroupChat.TryGetValue(groupChatId, out var members) &&
                members.Any(member => member.UserId == userId))
            {
                continue;
            }

            AddMember(groupChatId, userId);
            addedMembers.Add(_membersByGroupChat[groupChatId].Last());
        }

        return addedMembers;
    }

    public MemberResponseDto RemoveMember(int groupChatId, string userId, string currentUserId)
    {
        EnsureMember(groupChatId, currentUserId);

        if (_membersByGroupChat.TryGetValue(groupChatId, out var members))
        {
            var member = members.FirstOrDefault(candidate => candidate.UserId == userId);
            if (member is not null)
            {
                members.Remove(member);
                RemoveGroupChatForUser(groupChatId, userId);
                return member;
            }
        }

        return new MemberResponseDto();
    }

    public IEnumerable<MessageResponseDto> GetMessages(int groupChatId, string userId)
    {
        EnsureMember(groupChatId, userId);
        return _messagesByGroupChat.TryGetValue(groupChatId, out var messages)
            ? messages.AsEnumerable()
            : Enumerable.Empty<MessageResponseDto>();
    }

    public MessageResponseDto AddMessage(int groupChatId, string content, string userId)
    {
        EnsureMember(groupChatId, userId);

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

    public MessageResponseDto DeleteMessage(int groupChatId, int messageId, string userId)
    {
        EnsureMember(groupChatId, userId);

        if (_messagesByGroupChat.TryGetValue(groupChatId, out var messages))
        {
            var message = messages.FirstOrDefault(candidate =>
                candidate.Id == messageId && candidate.UserId == userId);

            if (message is not null)
            {
                messages.Remove(message);
                return message;
            }
        }

        throw new NotFoundException("Message not found.");
    }

    public MessageResponseDto UpdateMessage(
        int groupChatId,
        int messageId,
        MessageUpdateRequestDto request,
        string userId)
    {
        EnsureMember(groupChatId, userId);

        if (!_messagesByGroupChat.TryGetValue(groupChatId, out var messages))
        {
            throw new NotFoundException("Group chat not found.");
        }

        var message = messages.FirstOrDefault(candidate => candidate.Id == messageId);
        if (message is null)
        {
            throw new NotFoundException("Message not found.");
        }

        if (message.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to update this message.");
        }

        message.Content = request.Content;
        message.ModifiedAt = DateTime.UtcNow;
        return message;
    }

    private void AddMember(int groupChatId, string userId)
    {
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

        var groupChat = FindGroupChat(groupChatId);
        if (groupChat is not null)
        {
            AddGroupChatForUser(groupChat, userId);
        }
    }

    private void AddGroupChatForUser(GroupChatResponseDto groupChat, string userId)
    {
        if (!_groupChatsByUser.TryGetValue(userId, out var groupChats))
        {
            groupChats = new List<GroupChatResponseDto>();
            _groupChatsByUser[userId] = groupChats;
        }

        if (groupChats.All(candidate => candidate.Id != groupChat.Id))
        {
            groupChats.Add(groupChat);
        }
    }

    private void RemoveGroupChatForUser(int groupChatId, string userId)
    {
        if (_groupChatsByUser.TryGetValue(userId, out var groupChats))
        {
            groupChats.RemoveAll(groupChat => groupChat.Id == groupChatId);
        }
    }

    private GroupChatResponseDto GetRequiredGroupChat(int groupChatId, string userId)
    {
        return GetGroupChat(groupChatId, userId)
            ?? throw new NotFoundException("Group chat not found.");
    }

    private void EnsureMember(int groupChatId, string userId)
    {
        if (GetGroupChat(groupChatId, userId) is null)
        {
            throw new NotFoundException("Group chat not found for the current user.");
        }
    }

    private GroupChatResponseDto? FindGroupChat(int groupChatId)
    {
        return _groupChatsByUser.Values
            .SelectMany(groupChats => groupChats)
            .FirstOrDefault(groupChat => groupChat.Id == groupChatId);
    }
}
