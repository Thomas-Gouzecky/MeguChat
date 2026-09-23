using System.Text.Json.Serialization;

public class GroupChatResponseDto
{
    [JsonPropertyName("groupchat_id")]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}