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

namespace letiahomes.Application.Features.Auth.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                   ILogger<LoginCommandHandler> logger) {
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<ApiResult<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.login.Email);
            if (user == null)
            {
                return ApiResult<string>.Failure(
                    new CustomError("401", "Login Failed."));
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return ApiResult<string>.Failure( new CustomError("403","Login Failed ,Unverified Account."));
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.login.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return ApiResult<string>.Failure(new CustomError("403", "Unauthorized"));
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count <= 0)
            {
                return ApiResult<string>.Failure(new CustomError("400","You can not login at this time"));
            }


            throw new NotImplementedException();
        }
    }
}
