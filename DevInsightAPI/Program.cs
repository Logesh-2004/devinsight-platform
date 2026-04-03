using System.Text;
using DevInsightAPI.Data;
using DevInsightAPI.Hubs;
using DevInsightAPI.Persistence;
using DevInsightAPI.Repositories;
using DevInsightAPI.Services;
using DevInsightAPI.Services.AIInsights;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddProblemDetails();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrWhiteSpace(accessToken) &&
                    path.StartsWithSegments("/taskHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();

var useFileStorage = builder.Environment.IsDevelopment() &&
    string.Equals(builder.Configuration["Storage:Provider"], "File", StringComparison.OrdinalIgnoreCase);

if (useFileStorage)
{
    builder.Services.AddSingleton<FileWorkspaceStore>();
    builder.Services.AddScoped<IUserRepository, FileUserRepository>();
    builder.Services.AddScoped<IProjectRepository, FileProjectRepository>();
    builder.Services.AddScoped<ITaskRepository, FileTaskRepository>();
    builder.Services.AddScoped<INotificationRepository, FileNotificationRepository>();
    builder.Services.AddScoped<IAppDataSeeder, FileAppDataSeeder>();
}
else
{
    builder.Services.AddDbContext<DevInsightDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()));

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
    builder.Services.AddScoped<ITaskRepository, TaskRepository>();
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
    builder.Services.AddScoped<IAppDataSeeder, DbAppDataSeeder>();
}

builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddScoped<IPasswordService, Pbkdf2PasswordService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAIInsightsService, AIInsightsService>();
builder.Services.AddScoped<IAIInsightRule, DeveloperOverloadInsightRule>();
builder.Services.AddScoped<IAIInsightRule, DelayedTaskInsightRule>();
builder.Services.AddScoped<IAIInsightRule, ProjectRiskInsightRule>();
builder.Services.AddScoped<IAIInsightRule, ProductivityDropInsightRule>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            InvalidOperationException invalidOperationException => (
                StatusCodes.Status400BadRequest,
                invalidOperationException.Message),
            UnauthorizedAccessException unauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                unauthorizedAccessException.Message),
            SqlException => (
                StatusCodes.Status503ServiceUnavailable,
                "Database connection failed. DevInsight is configured for LocalDB in development, so please restart the API and try again."),
            _ => (
                StatusCodes.Status500InternalServerError,
                app.Environment.IsDevelopment()
                    ? $"{exception?.GetType().Name}: {exception?.Message}"
                    : "An unexpected server error occurred.")
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { message });
    });
});

if (app.Environment.IsDevelopment() && !useFileStorage)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<DevInsightDbContext>();
    dbContext.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IAppDataSeeder>();
    await seeder.EnsureSeedDataAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () =>
{
    if (app.Environment.IsDevelopment())
    {
        return Results.Redirect("/swagger");
    }

    return Results.Ok(new
    {
        name = "DevInsight API",
        status = "Running"
    });
});

app.MapControllers();
app.MapHub<TaskHub>("/taskHub");

app.Run();
