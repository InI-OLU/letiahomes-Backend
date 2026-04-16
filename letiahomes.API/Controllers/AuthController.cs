using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using letiahomes.Application.Features.Auth.Commands.BanUser;
using letiahomes.Application.Features.Auth.Commands.ForgottenPassword;
using letiahomes.Application.Features.Auth.Commands.Login;
using letiahomes.Application.Features.Auth.Commands.PasswordChange;
using letiahomes.Application.Features.Auth.Commands.RefreshToken;
using letiahomes.Application.Features.Auth.Commands.RegisterLandlord;
using letiahomes.Application.Features.Auth.Commands.RegisterTenant;
using letiahomes.Application.Features.Auth.Commands.ResendVerificationLink;
using letiahomes.Application.Features.Auth.Commands.ResetPassword;
using letiahomes.Application.Features.Auth.Commands.RevokeToken;
using letiahomes.Application.Features.Auth.Commands.VerifyOtp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace letiahomes.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("register/tenant")]
        public async Task<IActionResult> RegisterTenant(
            [FromBody] RegisterTenantRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RegisterTenantCommand(request), cancellationToken);

            return result.IsSuccess ? Ok(result) : Conflict(result);
        }

        [HttpPost("register/landlord")]
        public async Task<IActionResult> RegisterLandlord(
            [FromBody] RegisterLandlordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RegisterLandlordCommand(request), cancellationToken);

            return result.IsSuccess ? Ok(result) : Conflict(result);
        }

  
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(
            [FromBody] VerifyEmailRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new VerifyOtpCommand(request), cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


        [HttpPost("resend-verification-link")]
        public async Task<IActionResult> ResendVerificationLink(
            [FromBody] ResendVerificationLinkRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
               new ResendVerificationCommand( request), cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new LoginCommand(request), cancellationToken);

            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }



        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgottenPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new ForgottenPasswordCommand(request), cancellationToken);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new ResetPasswordCommand(request), cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


        [Authorize(Roles = "Admin,Landlord,Tenant")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var result = await _mediator.Send(
                new PasswordChangeCommand(userId, request), cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RefreshTokenCommand(request), cancellationToken);
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }
        [Authorize(Roles = "Admin,Landlord,Tenant")]
        [HttpPost("logout")]
        public async Task<IActionResult> RevokeToken(
            [FromBody] RevokeTokenRequest request,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
                new RevokeTokenCommand(userId), cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("ban-user/{userId}")]
        public async Task<IActionResult> BanUser( string userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new BanUserCommand(userId), cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}