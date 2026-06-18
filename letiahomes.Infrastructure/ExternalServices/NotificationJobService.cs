using Hangfire;
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

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10,20,30 })]
        public async Task ProcessWelcomeEmailAsync(WelcomeEmailPayload payload)
        {
            _logger.LogInformation("Processing welcome email job for {Recipient}", payload.Recipient);
            await _emailService.SendAccountVerificationAsync(payload.Recipient, payload.Subject, payload.Message);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessPasswordResetAsync(PasswordResetPayload payload)
        {
            _logger.LogInformation("Processing password reset email job for {Recipient}", payload.Recipient);
            await _emailService.SendPasswordResetAsync(payload.Recipient, payload.FirstName, payload.ResetLink);
        }

        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 15, 30, 60, 120, 300 })]
        public async Task ProcessAccountVerifiedAsync(AccountVerifiedPayload payload)
        {
            _logger.LogInformation("Processing account verified email job for {Recipient}", payload.Recipient);
            await _emailService.SendAccountVerifiedAsync(payload.Recipient, payload.FirstName, payload.LoginLink);
        }
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessBookingRequestedLandlordAsync(
    BookingRequestedLandlordPayload payload)
        {
            _logger.LogInformation(
                "Processing booking requested landlord email for {Recipient}",
                payload.Recipient);

            await _emailService.SendBookingRequestedLandlordEmailAsync(payload);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessBookingRequestedTenantAsync(
            BookingRequestedTenantPayload payload)
        {
            _logger.LogInformation(
                "Processing booking requested tenant email for {Recipient}",
                payload.Recipient);

            await _emailService.SendBookingRequestedTenantEmailAsync(payload);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessBookingConfirmedTenantAsync(
            BookingConfirmedTenantPayload payload)
        {
            _logger.LogInformation(
                "Processing booking confirmed tenant email for {Recipient}",
                payload.Recipient);

            await _emailService.SendBookingConfirmedTenantEmailAsync(payload);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessBookingConfirmedLandlordAsync(
            BookingConfirmedLandlordPayload payload)
        {
            _logger.LogInformation(
                "Processing booking confirmed landlord email for {Recipient}",
                payload.Recipient);

            await _emailService.SendBookingConfirmedLandlordEmailAsync(payload);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessBookingRejectedAsync(
            BookingRejectedPayload payload)
        {
            _logger.LogInformation(
                "Processing booking rejected email for {Recipient}",
                payload.Recipient);

            await _emailService.SendBookingRejectedEmailAsync(payload);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessBookingCancelledAsync(
            BookingCancelledPayload payload)
        {
            _logger.LogInformation(
                "Processing booking cancelled email for {Recipient}",
                payload.Recipient);

            await _emailService.SendBookingCancelledEmailAsync(payload);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessBookingCompletedTenantAsync(
            BookingCompletedTenantPayload payload)
        {
            _logger.LogInformation(
                "Processing booking completed tenant email for {Recipient}",
                payload.Recipient);

            await _emailService.SendBookingCompletedTenantEmailAsync(payload);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 20, 30 })]
        public async Task ProcessBookingCompletedLandlordAsync(
            BookingCompletedLandlordPayload payload)
        {
            _logger.LogInformation(
                "Processing booking completed landlord email for {Recipient}",
                payload.Recipient);

            await _emailService.SendBookingCompletedLandlordEmailAsync(payload);
        }
    }
}
