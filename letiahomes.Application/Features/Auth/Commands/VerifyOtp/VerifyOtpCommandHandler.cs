using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Notification;
using letiahomes.Application.Features.Auth.Commands.VerifyOtp;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public sealed class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, ApiResult<string>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly INotificationService _notificationService;
    private readonly ILogger<VerifyOtpCommandHandler> _logger;

    public VerifyOtpCommandHandler(
        UserManager<AppUser> userManager,
        IConfiguration configuration,
        INotificationService notificationService,
        ILogger<VerifyOtpCommandHandler> logger)  
    {
        _userManager = userManager;
        _configuration = configuration;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<ApiResult<string>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.request.UserId.ToString());
        if (user == null)
            return ApiResult<string>.Failure(new CustomError("404", "User not found"));

        var decodedToken = Uri.UnescapeDataString(request.request.OtpCode);
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            return ApiResult<string>.Failure(
       new CustomError("400", "Invalid or expired token"));
        }
        user.IsActive = true;
        user.IsVerified = true;
        await _userManager.UpdateAsync(user);

        var baseUrl = _configuration["AppSettings:FrontendUrl"]
            ?? throw new InvalidOperationException("BaseUrl is not configured.");

        var loginLink = $"{baseUrl}/auth/login";

        _notificationService.EnqueueAccountVerified(new AccountVerifiedPayload(
                user.Email!,
                user.FirstName,
                loginLink));
        _logger.LogInformation("A verification email has been resent to {UserId}", user.Id);
        return ApiResult<string>.Success("Account verified successfully. You can now log in.");
    }
}