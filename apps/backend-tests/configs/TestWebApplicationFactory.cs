using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"AuthTests-{Guid.NewGuid()}";
    private readonly TestDataStore _dataStore = new();

    public string TestUserId { get; private set; } = string.Empty;
    public string NoGroupChatsUserId { get; private set; } = string.Empty;

    public void ResetGroupChatState()
    {
        _dataStore.Reset(TestUserId, NoGroupChatsUserId);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.RemoveAll<IGroupChatClient>();
            services.RemoveAll<IMembersClient>();
            services.RemoveAll<IMessagesClient>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.AddSingleton(_dataStore);
            services.AddScoped<IGroupChatClient, FakeGroupChatClient>();
            services.AddScoped<IMembersClient, FakeMembersClient>();
            services.AddScoped<IMessagesClient, FakeMessagesClient>();
            services.Configure<IdentityOptions>(options =>
                options.SignIn.RequireConfirmedAccount = false);

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var testUser = SeedUser(userManager, "testuser");
            var noGroupChatsUser = SeedUser(userManager, "nouser");

            TestUserId = testUser.Id;
            NoGroupChatsUserId = noGroupChatsUser.Id;
            _dataStore.Reset(TestUserId, NoGroupChatsUserId);
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
