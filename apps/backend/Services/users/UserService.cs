using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService : IUsersService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                Username = user.UserName!
            })
            .ToListAsync();

        return users;
    }
}