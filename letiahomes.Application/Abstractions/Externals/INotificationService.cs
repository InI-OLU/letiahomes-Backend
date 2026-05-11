
using letiahomes.Application.DTOs.Notification;

namespace letiahomes.Application.Abstractions.Externals
{
    public interface INotificationService
    {
        void EnqueueAccountVerified(AccountVerifiedPayload payload);
        void EnqueuePasswordReset(PasswordResetPayload payload);
        void EnqueueWelcomeEmail(WelcomeEmailPayload payload);
    }
}
