using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using TatumConnectBackened.Auth;
using TatumConnectBackened.Common.Constants;
using TatumConnectBackened.Repositories;
using TatumConnectBackened.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TatumConnectBackened.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "TatumConnect API",
            Version = "v1",
            Description = "TatumConnect Digital Banking API"
        });

    //JWT BEARER AUTHENTICATION

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT access token"
        }
        );

    //Apply JWT Authentication To SWAGGER

    options.AddSecurityRequirement(
        document => new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)] = []
        }
        );
});




//egister repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IBillerRepository, BillerRepository>();
//builder.Services.AddScoped<ITransferRepository, TransferRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
// Register transaction services and current user service, and HTTP context accessor
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
//builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();


//egister services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
//builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
//services

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBillerService, BillerService>();

// Register JwtService so it can be injected into AuthService
builder.Services.AddScoped<TatumConnectBackened.Services.JwtService>();

builder.Services.AddScoped<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddHttpClient();
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("Sms"));
var jwtSettings =
    builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT settings are not configured properly.");
}

if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
{
    throw new InvalidOperationException("JWT secret is missing required values.");
}

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
            };
    }
);
builder.Services.AddAuthorization();
var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TatumConnect API v1");
    options.RoutePrefix = "swagger";
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

// Enable authentication middleware (JWT)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TatumConnectBackened.Data.AppDbContext>();
    await TatumConnectBackened.Data.DbSeeder.SeedAsync(dbContext);
}

app.Run();

//EF core