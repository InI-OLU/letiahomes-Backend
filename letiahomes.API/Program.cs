using FluentValidation;
using letiahomes.API.Extension;
using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.Common.Behaviours;
using letiahomes.Application.Settings;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using letiahomes.Infrastructure.ExternalServices;
using letiahomes.Infrastructure.RepsitoryManager;
using Mailjet.Client;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

// Bootstrap logger — catches startup crashes before full config loads
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Letia Homes API");

    var builder = WebApplication.CreateBuilder(args);
    var config = builder.Configuration;

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext());


    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        options.TokenLifespan = TimeSpan.FromMinutes(5));

   
    builder.Services.AddRateLimiter(options =>
    {
        options.AddPolicy("auth", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString(),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(5)
                }));
    });

    builder.Services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["JwtSettings:Issuer"],
            ValidAudience = config["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]
        ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.")))
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userManager = context.HttpContext.RequestServices
                    .GetRequiredService<UserManager<AppUser>>();

                var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    context.Fail("Unauthorized");
                    return;
                }

                var user = await userManager.FindByIdAsync(userId);

                if (user == null || !user.IsActive)
                {
                    context.Fail("User is inactive");
                }
            }
        };
    });

  
    var mailJetSection = config.GetSection("MailJet");
    builder.Services.Configure<MailjetSettings>(mailJetSection);
    var mailJetSettings = mailJetSection.Get<MailjetSettings>() ??
        throw new ArgumentNullException("MailJetSettings is not configured.");

    builder.Services.AddHttpClient<IMailjetClient, MailjetClient>(opt =>
        opt.UseBasicAuthentication(mailJetSettings.ApiKey, mailJetSettings.ApiSecret));

    builder.Services.Configure<AppSettings>(config.GetSection("AppSettings"));

    builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);

    // Register Cloudinary service
    builder.Services.AddSingleton<ICloudinaryService, CloudinaryService>();

    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(
            typeof(letiahomes.Application.AssemblyReference).Assembly));

    builder.Services.AddValidatorsFromAssembly(
        typeof(letiahomes.Application.AssemblyReference).Assembly);

    builder.Services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehaviour<,>));
   
    //Register  scoped Services
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
    builder.Services.AddScoped<ITokenExtension, TokenExtension>();

   
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();


    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Letia Homes API",
            Version = "v1",
            Description = "Property rental platform API"
        });

        // JWT Bearer button in Swagger UI
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter your JWT token like this: Bearer {your token}",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    });

 
    var app = builder.Build();
    app.UseSwagger();
    if (app.Environment.IsDevelopment())
    {
   
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Letia Homes API v1");
            options.DisplayRequestDuration();
        });
    }

    app.UseHttpsRedirection();
    app.UseRateLimiter();
    app.UseGlobalExceptionHandler();
    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Letia Homes API failed to start");
}
finally
{
    Log.CloseAndFlush();
}