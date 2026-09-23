public class CreateGroupChatRequestDto
{
    public string Name { get; set; } = string.Empty;
    public List<string> UserIds { get; set; } = new List<string>();
}