using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace letiahomes.Application.Features.Auth.Commands.PasswordChange
{
    public sealed class PasswordChangeCommandHandler
        : IRequestHandler<PasswordChangeCommand, ApiResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PasswordChangeCommandHandler> _logger;
        private readonly IRepositoryManager _repositoryManager;

        public PasswordChangeCommandHandler(
            UserManager<AppUser> userManager,
            ILogger<PasswordChangeCommandHandler> logger,
            IRepositoryManager repositoryManager)
        {
            _userManager = userManager;
            _logger = logger;
            _repositoryManager = repositoryManager;
        }

        public async Task<ApiResult<string>> Handle(
            PasswordChangeCommand request,
            CancellationToken cancellationToken)
        {
            // request.UserId should come from the authenticated user's claims
            // resolved in your controller via ICurrentUserService or User.GetUserId()
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return ApiResult<string>.Failure(
                    new CustomError("404", "User not found"));

            if (!user.IsActive)
                return ApiResult<string>.Failure(
                    new CustomError("403", "Your account has been suspended"));

            if (request.Request.NewPassword != request.Request.ConfirmNewPassword)
                return ApiResult<string>.Failure(
                    new CustomError("400", "New password and confirmation do not match"));

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.Request.CurrentPassword,
                request.Request.NewPassword);

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return ApiResult<string>.Failure(
                    new CustomError(error.Code, error.Description));
            }

            // Revoke all existing refresh tokens so all sessions are invalidated
            var userTokens = await _repositoryManager.RefreshTokens
                .FindAll(t => t.UserId == user.Id && !t.IsRevoked, false)
                .ToListAsync(cancellationToken);

            foreach (var token in userTokens)
                token.IsRevoked = true;

            await _userManager.UpdateSecurityStampAsync(user);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User {UserId} changed their password. {TokenCount} refresh token(s) revoked",
                user.Id, userTokens.Count);

            return ApiResult<string>.Success(
                "Password changed successfully. Please log in again.");
        }
    }
}