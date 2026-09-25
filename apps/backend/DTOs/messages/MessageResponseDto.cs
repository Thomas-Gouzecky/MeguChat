using System.Text.Json.Serialization;

public class MessageResponseDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("content")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;
    [JsonPropertyName("group_chat_id")]
    public int GroupChatId { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("modified_at")]
    public DateTime ModifiedAt { get; set; }
}