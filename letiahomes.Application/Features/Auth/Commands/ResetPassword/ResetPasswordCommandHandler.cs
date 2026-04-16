using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
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
        private readonly ILogger<ResetPasswordCommandHandler> _logger;
        private readonly IRepositoryManager _repositoryManager;

        public ResetPasswordCommandHandler(
            UserManager<AppUser> userManager,
            ILogger<ResetPasswordCommandHandler> logger,
            IRepositoryManager repositoryManager)
        {
            _userManager = userManager;
            _logger = logger;
            _repositoryManager = repositoryManager;
        }

        public async Task<ApiResult<string>> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Request.Email);
            if (user is null)
                return ApiResult<string>.Success(
                    "If this email exists, your password has been reset.");

            if (request.Request.NewPassword != request.Request.ConfirmNewPassword)
                return ApiResult<string>.Failure(
                    new CustomError("400", "New password and confirmation do not match"));

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

            var userTokens = await _repositoryManager.RefreshTokens.GetAllRefreshTokens(user.Id, cancellationToken);
               
            foreach (var token in userTokens)
                token.IsRevoked = true;

            await _userManager.UpdateSecurityStampAsync(user);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User {UserId} reset their password via email token. {TokenCount} refresh token(s) revoked",
                user.Id, userTokens.Count);

            return ApiResult<string>.Success(
                "Password reset successfully. Please log in with your new password.");
        }
    }
}