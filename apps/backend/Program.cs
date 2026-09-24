using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient();

var databaseApi = builder.Configuration["ApiSettings:DatabaseApi"]
    ?? throw new InvalidOperationException("Database API URL is missing");

builder.Services.AddHttpClient("dbApi", options =>
{
    options.BaseAddress = new Uri(databaseApi);
});

builder.Services.AddScoped<IGroupChatClient, GroupChatClient>();
builder.Services.AddScoped<IMembersService, MembersService>();
builder.Services.AddScoped<IMembersClient, MembersClient>();
builder.Services.AddScoped<IUserValidation, UserValidation>();
builder.Services.AddScoped<IMessagesClient, MessagesClient>();
builder.Services.AddScoped<IMessagesService, MessagesService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { } // Make the Program class public for integration testing