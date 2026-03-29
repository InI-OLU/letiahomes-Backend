using letiahomes.API.Extension;
using letiahomes.Application.Settings;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // Mailjet
    var mailJetSection = builder.Configuration.GetSection("MailJet");
    builder.Services.Configure<MailjetSettings>(mailJetSection);
    var mailJetSettings = mailJetSection.Get<MailjetSettings>() ??
        throw new ArgumentNullException("MailJetSettings");
    builder.Services.AddHttpClient<IMailjetClient, MailjetClient>(opt =>
    {
        opt.UseBasicAuthentication(mailJetSettings.ApiKey, mailJetSettings.ApiSecret);
    });

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