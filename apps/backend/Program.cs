using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGroupChatService, GroupChatService>();
builder.Services.AddScoped<IGroupChatClient, GroupChatClient>();

builder.Services.AddControllers();

builder.Services.AddProblemDetails();

builder.Services.AddHttpClient();

var databaseApi = builder.Configuration["ApiSettings:DatabaseApi"]
    ?? throw new InvalidOperationException("Database API URL is missing");

builder.Services.AddHttpClient("dbApi", options =>
{
    options.BaseAddress = new Uri(databaseApi);
});

// builder.Services.AddSingleton<IAuthService, AuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (dbContext.Database.IsRelational())
    {
        await dbContext.Database.MigrateAsync();
    }
}

if (args.Contains("--migrate", StringComparer.OrdinalIgnoreCase))
{
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { } // Make the Program class public for integration testing