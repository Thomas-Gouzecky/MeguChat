public interface IUserValidation
{
    Task<GroupChatResponseDto> EnsureUserIsMember(string userId, int groupChatId);
    Task<ApplicationUser> ValidateUser();
}