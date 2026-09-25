using System.Text.Json.Serialization;

public class GroupChatResponseDto : IGroupChatRequest
{
    [JsonPropertyName("groupchat_id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}