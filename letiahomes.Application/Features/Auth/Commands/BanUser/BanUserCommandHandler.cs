using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Auth.Commands.BanUser
{
    public sealed class BanUserCommandHandler
        : IRequestHandler<BanUserCommand, ApiResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<BanUserCommandHandler> _logger;
        private readonly IRepositoryManager _repositoryManager;

        public BanUserCommandHandler(
            UserManager<AppUser> userManager,
            ILogger<BanUserCommandHandler> logger,
            IRepositoryManager repositoryManager)
        {
            _userManager = userManager;
            _logger = logger;
            _repositoryManager = repositoryManager;
        }

        public async Task<ApiResult<string>> Handle(
            BanUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return ApiResult<string>.Failure(
                    new CustomError("404", "User not found"));

            if (!user.IsActive)
                return ApiResult<string>.Failure(
                    new CustomError("400", "User already banned"));


            user.IsActive = false;

            await _userManager.UpdateSecurityStampAsync(user);

            var tokens = await _repositoryManager.RefreshTokens.GetAllRefreshTokens(user.Id, cancellationToken);
                

            foreach (var token in tokens)
                token.IsRevoked = true;

            await _repositoryManager.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("User {UserId} has been banned", user.Id);

            return ApiResult<string>.Success("User banned successfully");
        }
    }
}
