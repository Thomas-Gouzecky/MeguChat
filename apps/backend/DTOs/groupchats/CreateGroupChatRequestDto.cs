using System.Text.Json.Serialization;

public class CreateGroupChatRequestDto : IGroupChatRequest
{
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("user_ids")]
    public List<string>? UserIds { get; set; }
}