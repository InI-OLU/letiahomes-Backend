using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.Features.Auth.Commands.RevokeToken;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public sealed class RevokeTokenCommandHandler
    : IRequestHandler<RevokeTokenCommand, ApiResult<string>>
{

    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<RevokeTokenCommandHandler> _logger;
    private readonly IRepositoryManager _repositoryManager;

    public RevokeTokenCommandHandler(
        UserManager<AppUser> userManager,
        ILogger<RevokeTokenCommandHandler> logger,
        IRepositoryManager repositoryManager)
    {
        _userManager = userManager;
        _logger = logger;
        _repositoryManager = repositoryManager;
    }

    public async Task<ApiResult<string>> Handle(
        RevokeTokenCommand request,
        CancellationToken cancellationToken)
    {

       
        var token = await _repositoryManager.RefreshTokens.GetRefreshToken(request.UserId, cancellationToken);

        if (token == null)
        {
            return ApiResult<string>.Failure(
                new CustomError("404", "Refresh token not found"));
        }

        if (token.IsRevoked)
        {
            return ApiResult<string>.Failure(
                new CustomError("400", "Token already revoked"));
        }


        var user = await _userManager.FindByIdAsync(token.UserId);

        if (user == null)
        {
            return ApiResult<string>.Failure(
                new CustomError("404", "User not found"));
        }
        var tokens = await _repositoryManager.RefreshTokens.GetAllRefreshTokens(user.Id, cancellationToken); 

       foreach (var t in tokens)
        {
            t.IsRevoked = true;
        }

        await _userManager.UpdateSecurityStampAsync(user);

        await _repositoryManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} logged out", user.Id);

        return ApiResult<string>.Success("Logged out successfully");
    }
}