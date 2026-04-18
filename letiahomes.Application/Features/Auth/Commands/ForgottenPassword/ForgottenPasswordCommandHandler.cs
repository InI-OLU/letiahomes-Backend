using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace letiahomes.Application.Features.Auth.Commands.ForgottenPassword
{
    public class ForgottenPasswordCommandHandler : IRequestHandler<ForgottenPasswordCommand, ApiResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgottenPasswordCommandHandler> _logger;
        private readonly IOptions<AppSettings> _options;

        public ForgottenPasswordCommandHandler(UserManager<AppUser> user,IEmailService emailService,
                                               ILogger<ForgottenPasswordCommandHandler> logger,
                                               IOptions<AppSettings> options)
        {
            _userManager = user;
            _emailService = emailService;
            _logger = logger;
            _options = options;
        }
        public async Task<ApiResult<string>> Handle(ForgottenPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.request.Email);
            if (user == null)
            {
                _logger.LogWarning("Password reset requested for non-existent email {Email}", request.request.Email);
                return ApiResult<string>.Success("If this email exists you will receive a reset link shortly");
            }

            if (!user.IsVerified)
                return ApiResult<string>.Failure(
                    new CustomError("403", "Please verify your email before logging in"));

            if (!user.IsActive)
                return ApiResult<string>.Failure(
                    new CustomError("403", "Your account has been suspended"));
            var frontendUrl = _options.Value.FrontendUrl;
            var encodedToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{frontendUrl}/auth/reset-password?token={encodedToken}&email={Uri.EscapeDataString(user.Email!)}";
            var emailSent = await _emailService.SendPasswordResetAsync(
                         user.Email!,
                         user.FirstName,
                         resetLink
                                   );

            if (!emailSent)
            {
                _logger.LogError("Failed to send password reset email to {UserId}", user.Id);
                return ApiResult<string>.Failure(
                    new CustomError("500", "Failed to send reset email. Please try again"));
            }

            _logger.LogInformation("Password reset email sent to {UserId}", user.Id);
            _logger.LogInformation("Password reset token sent : {Token}", encodedToken);
            return ApiResult<string>.Success("If this email exists you will receive a reset link shortly");
        }
    }
}
