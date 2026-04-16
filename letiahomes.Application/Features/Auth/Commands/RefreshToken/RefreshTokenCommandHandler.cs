using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RefreshTokenEntity = letiahomes.Domain.Entities.RefreshToken;

namespace letiahomes.Application.Features.Auth.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler
        : IRequestHandler<RefreshTokenCommand, ApiResult<TokenResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenExtension _tokenExtension;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly IRepositoryManager _repositoryManager;

        public RefreshTokenCommandHandler(
            UserManager<AppUser> userManager,
            ITokenExtension tokenExtension,
            ILogger<RefreshTokenCommandHandler> logger,
            IRepositoryManager repositoryManager)
        {
            _userManager = userManager;
            _tokenExtension = tokenExtension;
            _logger = logger;
            _repositoryManager = repositoryManager;
        }

        public async Task<ApiResult<TokenResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            
            var storedToken = await _repositoryManager.RefreshTokens.GetRefreshToken(request.request.RefreshToken, cancellationToken);

            if (storedToken == null)
                return ApiResult<TokenResponse>.Failure(
                    new CustomError("401", "Invalid refresh token"));

            if (storedToken.IsRevoked)
                return ApiResult<TokenResponse>.Failure(
                    new CustomError("401", "Refresh token has already been revoked"));

            if (storedToken.ExpiryDate < DateTime.UtcNow)
                return ApiResult<TokenResponse>.Failure(
                    new CustomError("401", "Refresh token has expired"));

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                return ApiResult<TokenResponse>.Failure(
                    new CustomError("404", "User not found"));

            if (!user.IsActive || !user.IsVerified)
                return ApiResult<TokenResponse>.Failure(
                    new CustomError("403", "Account is not active or verified"));

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenExtension.GenerateAccessToken(user, roles);

           
            storedToken.IsRevoked = true;

            
            var newRefreshToken = new RefreshTokenEntity
            {
                Token = _tokenExtension.GenerateRefreshToken(),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _repositoryManager.RefreshTokens.AddAsync(newRefreshToken);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} refreshed tokens successfully", user.Id);

            return ApiResult<TokenResponse>.Success(
                new TokenResponse(newAccessToken, newRefreshToken.Token));
        }
    }
}