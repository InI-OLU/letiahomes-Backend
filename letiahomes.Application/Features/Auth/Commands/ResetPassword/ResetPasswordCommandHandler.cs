using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace letiahomes.Application.Features.Auth.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler
        : IRequestHandler<ResetPasswordCommand, ApiResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            UserManager<AppUser> userManager,
            IApplicationDbContext context,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResult<string>> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            // Intentionally vague lookup response to prevent user enumeration attacks
            var user = await _userManager.FindByEmailAsync(request.Request.Email);
            if (user is null)
                return ApiResult<string>.Success(
                    "If this email exists, your password has been reset.");

            if (request.Request.NewPassword != request.Request.ConfirmNewPassword)
                return ApiResult<string>.Failure(
                    new CustomError("400", "New password and confirmation do not match"));

            // Token comes from the email link — Identity validates it internally
            var result = await _userManager.ResetPasswordAsync(
                user,
                request.Request.Token,
                request.Request.NewPassword);

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return ApiResult<string>.Failure(
                    new CustomError(error.Code, error.Description));
            }

            // Revoke all existing refresh tokens so all sessions are invalidated
            var userTokens = await _context.RefreshTokens
                .Where(t => t.UserId == user.Id && !t.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in userTokens)
                token.IsRevoked = true;

            // Invalidate all cookie-based sessions and JWT security stamp checks
            await _userManager.UpdateSecurityStampAsync(user);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User {UserId} reset their password via email token. {TokenCount} refresh token(s) revoked",
                user.Id, userTokens.Count);

            return ApiResult<string>.Success(
                "Password reset successfully. Please log in with your new password.");
        }
    }
}