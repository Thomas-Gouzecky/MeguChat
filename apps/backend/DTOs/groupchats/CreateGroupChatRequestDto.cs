using System.Text.Json.Serialization;

public class CreateGroupChatRequestDto : IGroupChatRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("users")]
    public List<string>? UserIds { get; set; }
}