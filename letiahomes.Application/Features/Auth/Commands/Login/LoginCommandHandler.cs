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

namespace letiahomes.Application.Features.Auth.Commands.Login
{
    public sealed class LoginCommandHandler
        : IRequestHandler<LoginCommand, ApiResult<TokenResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenExtension _tokenExtension;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IRepositoryManager _repositoryManager;

        public LoginCommandHandler(
            UserManager<AppUser> userManager,
            ITokenExtension tokenExtension,
            ILogger<LoginCommandHandler> logger,
            IRepositoryManager repositoryManager)
        {
            _userManager = userManager;
            _tokenExtension = tokenExtension;
            _logger = logger;
            _repositoryManager = repositoryManager;
        }

        public async Task<ApiResult<TokenResponse>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.login.Email);
            if (user == null)
                return ApiResult<TokenResponse>.Failure(
                    new CustomError("401", "Invalid email or password"));

            if (!user.IsVerified)
                return ApiResult<TokenResponse>.Failure(
                    new CustomError("403", "Please verify your email before logging in"));

            if (!user.IsActive)
                return ApiResult<TokenResponse>.Failure(
                    new CustomError("403", "Your account has been suspended"));

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.login.Password);
            if (!passwordValid)
            {

                await _userManager.AccessFailedAsync(user);
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("User {UserId} has been locked out after failed attempts", user.Id);
                    return ApiResult<TokenResponse>.Failure(
                        new CustomError("423", "Account locked due to multiple failed attempts. Try again later"));
                }

                return ApiResult<TokenResponse>.Failure(
                    new CustomError("401", "Invalid email or password"));
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenExtension.GenerateAccessToken(user, roles);
            var refreshToken = new RefreshTokenEntity
            {
                Token = _tokenExtension.GenerateRefreshToken(),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            var existingTokens = await _repositoryManager.RefreshTokens
                .FindAll(x => x.UserId == user.Id && !x.IsRevoked,false)
                .ToListAsync(cancellationToken);

            foreach (var token in existingTokens)
                token.IsRevoked = true;

            await _repositoryManager.RefreshTokens.AddAsync(refreshToken);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return ApiResult<TokenResponse>.Success(
                new TokenResponse(accessToken, refreshToken.Token));
        }
    }
}