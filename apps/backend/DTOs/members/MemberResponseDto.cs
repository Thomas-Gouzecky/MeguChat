using System.Text.Json.Serialization;

public class MemberResponseDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("group_chat_id")]
    public int GroupChatId { get; set; }
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;
    [JsonPropertyName("joined_at")]
    public DateTime JoinedAt { get; set; }
    [JsonPropertyName("last_active_at")]
    public DateTime LastActiveAt { get; set; }
    [JsonPropertyName("last_read_message_id")]
    public int? LastReadMessageId { get; set; }
}