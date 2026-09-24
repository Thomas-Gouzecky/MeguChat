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

    public async Task<MessageResponseDto> SendMessageToGroupChatAsync(int groupChatId, MessageCreationRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userValidation.ValidateUser();
        await _userValidation.EnsureUserIsMember(user.Id, groupChatId);

        var message = await _messagesClient.SendMessageToGroupChatAsync(groupChatId, request, user.Id, cancellationToken);
        return message;
    }

    public async Task<bool> DeleteMessageByIdAsync(int groupChatId, int messageId, CancellationToken cancellationToken = default)
    {
        var user = await _userValidation.ValidateUser();
        await _userValidation.EnsureUserIsMember(user.Id, groupChatId);

        return await _messagesClient.DeleteMessageFromGroupChatAsync(groupChatId, messageId, user.Id, cancellationToken);
    }

    public async Task<MessageResponseDto> UpdateMessageByIdAsync(int groupChatId, int messageId, MessageUpdateRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userValidation.ValidateUser();
        await _userValidation.EnsureUserIsMember(user.Id, groupChatId);

        var updatedMessage = await _messagesClient.UpdateMessageInGroupChatAsync(groupChatId, messageId, request, user.Id, cancellationToken);
        return updatedMessage;
    }
}