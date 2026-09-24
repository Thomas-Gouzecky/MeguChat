public class MemberResponseDto
{
    public int Id { get; set; }
    public int GroupChatId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public DateTime LastActiveAt { get; set; }
    public int LastReadMessageId { get; set; }
}