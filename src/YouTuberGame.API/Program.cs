using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

// Serilog 설정
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    Log.Information("🚀 YouTuber Game API Server Starting...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog 사용
    builder.Host.UseSerilog();

    // CORS 설정
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Controllers 추가
    builder.Services.AddControllers();

    // Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Database Context
    builder.Services.AddDbContext<YouTuberGame.API.Data.GameDbContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(8, 0, 21))
        ));

    // Services 등록
    builder.Services.AddScoped<YouTuberGame.API.Services.AuthService>();
    builder.Services.AddScoped<YouTuberGame.API.Services.GachaService>();
    builder.Services.AddScoped<YouTuberGame.API.Services.CharacterService>();
    builder.Services.AddScoped<YouTuberGame.API.Services.ContentService>();
    builder.Services.AddScoped<YouTuberGame.API.Services.EquipmentService>();
    builder.Services.AddScoped<YouTuberGame.API.Services.RankingService>();

    // JWT 인증
    var jwtKey = builder.Configuration["Jwt:Key"];
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"];

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
            };

            // 로깅 추가
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Log.Information("JWT Token validated for user: {User}", context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // HTTP 요청 파이프라인 설정
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowAll");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // 헬스 체크 엔드포인트
    app.MapGet("/health", () =>
    {
        Log.Information("Health check requested");
        return Results.Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }).WithName("HealthCheck");

    Log.Information("✅ YouTuber Game API Server Started Successfully!");
    Log.Information("📍 Environment: {Environment}", app.Environment.EnvironmentName);
    Log.Information("🌐 Listening on: {Urls}", string.Join(", ", builder.WebHost.GetSetting("urls") ?? "http://localhost:5000"));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
