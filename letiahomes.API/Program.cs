using FluentValidation;
using letiahomes.API.Extension;
using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common.Behaviours;
using letiahomes.Application.Settings;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using letiahomes.Infrastructure.ExternalServices;
using Mailjet.Client;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.RateLimiting;

// Bootstrap logger — catches startup crashes before full config loads
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Letia Homes API");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog — reads from appsettings.json "Serilog" section
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext());

    // Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Identity
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

    //ratelimiting 
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

    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        options.TokenLifespan = TimeSpan.FromMinutes(5);
    });

    //jwt token 
    builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]))
        };
    });

    // Mailjet
    var mailJetSection = builder.Configuration.GetSection("MailJet");
    builder.Services.Configure<MailjetSettings>(mailJetSection);
    var mailJetSettings = mailJetSection.Get<MailjetSettings>() ??
        throw new ArgumentNullException("MailJetSettings");
    builder.Services.AddHttpClient<IMailjetClient, MailjetClient>(opt =>
    {
        opt.UseBasicAuthentication(mailJetSettings.ApiKey, mailJetSettings.ApiSecret);
    });

    //appsetting
    builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));
    // Mediatr
    builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(letiahomes.Application.AssemblyReference).Assembly));

    builder.Services.AddValidatorsFromAssembly(
    typeof(letiahomes.Application.AssemblyReference).Assembly);

    builder.Services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehaviour<,>));
    //Services
        builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

    // Controllers
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
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