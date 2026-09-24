public class MessagesService : IMessagesService
{
    private readonly IMessagesClient _messagesClient;
    private readonly IUserValidation _userValidation;

    public MessagesService(IMessagesClient messagesClient, IUserValidation userValidation)
    {
        _messagesClient = messagesClient;
        _userValidation = userValidation;
    }

    public async Task<IEnumerable<MessageResponseDto>> GetMessagesOfGroupChatAsync(int groupChatId, CancellationToken cancellationToken = default)
    {
        var user = await _userValidation.ValidateUser();
        await _userValidation.EnsureUserIsMember(user.Id, groupChatId);

        var messages = await _messagesClient.GetMessagesOfGroupChatAsync(groupChatId, cancellationToken);
        return messages;
    }
}