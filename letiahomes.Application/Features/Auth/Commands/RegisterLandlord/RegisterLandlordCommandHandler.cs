using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using letiahomes.Application.DTOs.Notification;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace letiahomes.Application.Features.Auth.Commands.RegisterLandlord
{
    public sealed class RegisterLandlordCommandHandler
        : IRequestHandler<RegisterLandlordCommand, ApiResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterLandlordCommandHandler> _logger;
        private readonly IHostEnvironment _host;
        private readonly IConfiguration _configuration;
        private readonly IOptions<AppSettings> _options;
        private readonly IRepositoryManager _repositoryManager;
        private readonly INotificationService _notificationService;

        public RegisterLandlordCommandHandler(
            UserManager<AppUser> userManager,
            ILogger<RegisterLandlordCommandHandler> logger,
            IHostEnvironment host,
            IConfiguration configuration,
            IOptions<AppSettings> options,
            IRepositoryManager repositoryManager,
            INotificationService notificationService)
        {
            _userManager = userManager;
            _logger = logger;
            _host = host;
            _configuration = configuration;
            _options = options;
            _repositoryManager = repositoryManager;
            _notificationService = notificationService;
        }

        public async Task<ApiResult<string>> Handle(
            RegisterLandlordCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Request;

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return ApiResult<string>.Failure(
                    new CustomError("409", "A user with this email already exists."));
            }

            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IsActive = false,
                IsVerified = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return ApiResult<string>.Failure(
                    new CustomError(error.Code, error.Description));
            }

            
            await _userManager.AddToRoleAsync(user, "Landlord");

            
            var landlordProfile = new LandlordProfile
            {
                AppUserId = user.Id,
            };

            await _repositoryManager.Landlords.AddAsync(landlordProfile);
            _logger.LogInformation("Landlord registered successfully: {Email}", user.Email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var frontendUrl = _options.Value.FrontendUrl;

            var confirmationLink = $"{frontendUrl}/confirm-email" +
                                   $"?userId={user.Id}&token={encodedToken}";

            var message = GetAccountVerificationMessage(user.FirstName, confirmationLink);

            _notificationService.EnqueueWelcomeEmail(new WelcomeEmailPayload
                                (
                                     user.Email,
                                     user.FirstName,
                                     message
                                ));
            _logger.LogInformation("Welcome email sent to {UserId}", user.Id);
            _logger.LogInformation(
                                 """
                                Email Confirmation Details:
                                 Email: {Email}
                                UserId: {UserId}
                                Token: {Token}
                                """,
                                 user.Email,
                                 user.Id,
                                 token
                                );
            await _repositoryManager.SaveChangesAsync(cancellationToken);
            return ApiResult<string>.Success(
                "Registration successful. Please check your email to verify your account.");
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