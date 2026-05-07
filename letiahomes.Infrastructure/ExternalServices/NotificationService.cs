using Hangfire;
using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.DTOs.Notification;


namespace letiahomes.Infrastructure.ExternalServices
{
   public sealed class NotificationService:INotificationService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public NotificationService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public void EnqueueWelcomeEmail(WelcomeEmailPayload payload)
            => _backgroundJobClient.Enqueue<NotificationJobService>(
                job => job.ProcessWelcomeEmailAsync(payload));

        public void EnqueuePasswordReset(PasswordResetPayload payload)
      => _backgroundJobClient.Enqueue<NotificationJobService>(
          job => job.ProcessPasswordResetAsync(payload));

        public void EnqueueAccountVerified(AccountVerifiedPayload payload)
            => _backgroundJobClient.Enqueue<NotificationJobService>(
                job => job.ProcessAccountVerifiedAsync(payload));
    }
}
