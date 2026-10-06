public interface IUsersService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
}