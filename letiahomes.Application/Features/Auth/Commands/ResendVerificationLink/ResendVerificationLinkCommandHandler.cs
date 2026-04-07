using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace letiahomes.Application.Features.Auth.Commands.ResendVerificationLink
{
    public sealed class ResendVerificationCommandHandler
     : IRequestHandler<ResendVerificationCommand, ApiResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public ResendVerificationCommandHandler(
            UserManager<AppUser> userManager,
            IEmailService emailService,
            IConfiguration config)
        {
            _userManager = userManager;
            _emailService = emailService;
            _config = config;
        }

        public async Task<ApiResult<string>> Handle(
            ResendVerificationCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.request.Email);

            if (user == null || user.EmailConfirmed)
            {
                return ApiResult<string>.Success(
                    "If the account exists, a verification email has been sent.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var frontendUrl = _config["AppSettings:FrontendUrl"];

            var link = $"{frontendUrl}/confirm-email" +
                       $"?userId={user.Id}&token={encodedToken}";

            await _emailService.SendAsync(
                user.Email!,
                "Verify your account",
                $"Click here: {link}");

            return ApiResult<string>.Success(
                "If the account exists, a verification email has been sent.");
        }
    }
}
