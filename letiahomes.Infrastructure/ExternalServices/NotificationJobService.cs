using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.DTOs.Notification;
using Microsoft.Extensions.Logging;


namespace letiahomes.Infrastructure.ExternalServices
{
    public class NotificationJobService
    {
        private readonly ILogger<NotificationJobService> _logger;
        private readonly IEmailService _emailService;

        public NotificationJobService(ILogger<NotificationJobService> logger,IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task ProcessWelcomeEmailAsync(WelcomeEmailPayload payload)
        {
            _logger.LogInformation("Processing welcome email job for {Recipient}", payload.Recipient);
            await _emailService.SendAsync(payload.Recipient, payload.Subject, payload.Message);
        }

        public async Task ProcessPasswordResetAsync(PasswordResetPayload payload)
        {
            _logger.LogInformation("Processing password reset email job for {Recipient}", payload.Recipient);
            await _emailService.SendPasswordResetAsync(payload.Recipient, payload.FirstName, payload.ResetLink);
        }

        public async Task ProcessAccountVerifiedAsync(AccountVerifiedPayload payload)
        {
            _logger.LogInformation("Processing account verified email job for {Recipient}", payload.Recipient);
            await _emailService.SendAccountVerifiedAsync(payload.Recipient, payload.FirstName, payload.LoginLink);
        }
    }
}
