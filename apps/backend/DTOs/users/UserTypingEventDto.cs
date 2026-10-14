using System.Text.Json.Serialization;

public class UserTypingEventDto
{
    [JsonPropertyName("groupchat_id")]
    public int GroupchatId { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;
}