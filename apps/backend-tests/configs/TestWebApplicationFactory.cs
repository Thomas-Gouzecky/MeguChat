using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"AuthTests-{Guid.NewGuid()}";
    private readonly Mock<IGroupChatClient> _groupChatClient = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the application's real database registration
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.RemoveAll<IGroupChatClient>();
            services.AddScoped(_ => _groupChatClient.Object);
            services.Configure<IdentityOptions>(options =>
                options.SignIn.RequireConfirmedAccount = false);

            // Add test database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var testUser = SeedUser(userManager, "testuser");
            var noGroupChatsUser = SeedUser(userManager, "nouser");

            _groupChatClient
                .Setup(client => client.GetGroupChatsForUserAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new GroupChatResponseDto
                    {
                        Id = 1,
                        Name = "Test Group Chat",
                        CreatedAt = DateTime.UtcNow
                    }
                }.AsEnumerable());

            _groupChatClient
                .Setup(client => client.GetGroupChatsForUserAsync(
                    noGroupChatsUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<GroupChatResponseDto>());
        });
    }

    private static ApplicationUser SeedUser(
        UserManager<ApplicationUser> userManager,
        string username)
    {
        var existingUser = userManager.FindByNameAsync(username)
            .GetAwaiter()
            .GetResult();

        if (existingUser is not null)
        {
            return existingUser;
        }

        var user = new ApplicationUser
        {
            UserName = username,
            Email = $"{username}@example.com",
            EmailConfirmed = true
        };
        var seedResult = userManager.CreateAsync(user, "TestPassword1!")
            .GetAwaiter()
            .GetResult();

        if (!seedResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", seedResult.Errors.Select(error => error.Description)));
        }

        return user;
    }
}