using letiahomes.Application.Common;
using letiahomes.Application.Features.Auth.Commands.RevokeToken;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class RevokeTokenCommandHandler
    : IRequestHandler<RevokeTokenCommand, ApiResult<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<RevokeTokenCommandHandler> _logger;

    public RevokeTokenCommandHandler(
        IApplicationDbContext context,
        UserManager<AppUser> userManager,
        ILogger<RevokeTokenCommandHandler> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiResult<string>> Handle(
        RevokeTokenCommand request,
        CancellationToken cancellationToken)
    {

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request. request.RefreshToken, cancellationToken);

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
        token.IsRevoked = true;

        await _userManager.UpdateSecurityStampAsync(user);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} logged out", user.Id);

        return ApiResult<string>.Success("Logged out successfully");
    }
}