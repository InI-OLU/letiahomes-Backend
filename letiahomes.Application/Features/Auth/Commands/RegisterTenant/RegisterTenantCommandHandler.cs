using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Auth.Commands.RegisterTenant
{
    public sealed class RegisterTenantCommandHandler
     : IRequestHandler<RegisterTenantCommand, ApiResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisterTenantCommandHandler> _logger;
        private readonly IHostEnvironment _host;
        private readonly IConfiguration _configuration;
        private readonly IOptions<AppSettings> _options;

        public RegisterTenantCommandHandler(
            UserManager<AppUser> userManager,
            IApplicationDbContext context,
            IEmailService emailService,
            ILogger<RegisterTenantCommandHandler> logger,
            IHostEnvironment host,
            IConfiguration configuration,
            IOptions<AppSettings> options)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _host = host;
            _configuration = configuration;
            _options = options.Value;
        }

        public async Task<ApiResult<string>> Handle(
            RegisterTenantCommand request,
            CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Request.Email);
            if (existingUser == null)
            {
                return ApiResult<string>.Failure(
                    new CustomError("409", "A user with this email already exists."));
            }

            var user = new AppUser
            {
                FirstName = request.Request.FirstName,
                LastName = request.Request.LastName,
                Email = request.Request.Email,
                UserName = request.Request.Email,
                PhoneNumber = request.Request.PhoneNumber,
                IsActive = false,    
                IsVerified = false
            };

            var result = await _userManager.CreateAsync(user, request.Request.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return ApiResult<string>.Failure(
                    new CustomError(error.Code, error.Description));
            }
            await _userManager.AddToRoleAsync(user, "Tenant");
            var tenantProfile = new TenantProfile
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TenantProfiles.AddAsync(tenantProfile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Tenant registered successfully: {Email}", user.Email);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var frontendUrl = _appSettings.FrontendUrl;

            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                _logger.LogError("Frontend URL not configured");

                return ApiResult<string>.Failure(
                    new CustomError("500", "Server configuration error"));
            }
            var confirmationLink = $"{FrontendUrl}/confirm-email" +
                                   $"?userId={user.Id}&token={encodedToken}";
            var message = GetAccountVerificationMessage(user.FirstName, confirmationLink);

            await _emailService.SendAsync(user.Email, "Confirm your account", message);

            return ApiResult<string>.Success("Registration successful. Please check your email to verify your account.");
        }

        private string GetAccountVerificationMessage(string firstName, string link)
        {
            var path = Path.Combine(_host.ContentRootPath, "wwwroot", "EmailTemplate", "AccountVerification.html");

            if (File.Exists(path))
            {
                var template = File.ReadAllText(path);

                return template
                    .Replace("{{FirstName}}", firstName)
                    .Replace("{{CONFIRMATION_LINK}}", link);
            }

            throw new Exception($"Path not found: {path}");
        }
    }
}
