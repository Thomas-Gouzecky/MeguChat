using DotNetEnv;
using Anthropic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

var envPath = Path.Combine(
    AppContext.BaseDirectory,
    ".env.local");

Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);
var isMigration = args.Contains("--migrate", StringComparer.OrdinalIgnoreCase);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

if (!isMigration)
{
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager();
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IGroupChatService, GroupChatService>();
    builder.Services.AddScoped<IGroupChatClient, GroupChatClient>();
    builder.Services.AddScoped<IMembersService, MembersService>();
    builder.Services.AddScoped<IMembersClient, MembersClient>();
    builder.Services.AddScoped<IUserValidation, UserValidation>();
    builder.Services.AddScoped<IMessagesClient, MessagesClient>();
    builder.Services.AddScoped<IMessagesService, MessagesService>();
    builder.Services.AddScoped<IUsersService, UserService>();

    builder.Services.AddControllers();
    builder.Services.AddExceptionHandler<DbApiExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddHttpClient();

    var databaseApi = builder.Configuration["ApiSettings:DatabaseApi"]
        ?? throw new InvalidOperationException("Database API URL is missing");
    var redisConnectionString = builder.Configuration["ApiSettings:Redis"]
        ?? throw new InvalidOperationException("Redis connection string is missing");

    builder.Services.AddHttpClient("dbApi", options =>
    {
        options.BaseAddress = new Uri(databaseApi);
    });

    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
    }).AddStackExchangeRedis(redisConnectionString);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("NextFrontendDevelopmentCorsPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });

        options.AddPolicy("NextFrontendProductionCorsPolicy", policy =>
        {
            // subject to change
            // localhost:3000 is for testing the production build locally, but in production this should be the actual domain of the frontend
            policy.WithOrigins("https://meguchat.vercel.app", "http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    // AI Orchestrator and Chat Client Configuration

    var apiKey = builder.Configuration["Anthropic_API_KEY"] ?? throw new InvalidOperationException("Anthropic API key is missing");

    var claudeClient = new AnthropicClient
    {
        ApiKey = apiKey
    };

    builder.Services.AddSingleton<IChatClient>(
        claudeClient
            .AsIChatClient("claude-sonnet-5-5")
            .AsBuilder()
            .UseFunctionInvocation()
            .Build()
    );
    builder.Services.AddScoped<IMCPClient, MCPClient>();
    builder.Services.AddScoped<IAIOrchestrator, AIOrchestrator>();
    builder.Services.AddScoped<IAIService, AIService>();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (dbContext.Database.IsRelational())
    {
        await dbContext.Database.MigrateAsync();
    }
}

if (isMigration)
{
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(app.Environment.IsDevelopment() ? "NextFrontendDevelopmentCorsPolicy" : "NextFrontendProductionCorsPolicy");

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

app.Run();

public partial class Program { } // Make the Program class public for integration testing