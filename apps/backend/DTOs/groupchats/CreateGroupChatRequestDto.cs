public class CreateGroupChatRequestDto : IGroupChatRequest
{
    public string Name { get; set; } = string.Empty;
    public List<string> UserIds { get; set; } = new List<string>();
}