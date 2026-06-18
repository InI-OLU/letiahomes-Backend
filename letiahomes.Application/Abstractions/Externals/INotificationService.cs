
using letiahomes.Application.DTOs.Notification;

namespace letiahomes.Application.Abstractions.Externals
{
    public interface INotificationService
    {
        void EnqueueAccountVerified(AccountVerifiedPayload payload);
        void EnqueuePasswordReset(PasswordResetPayload payload);
        void EnqueueWelcomeEmail(WelcomeEmailPayload payload);
        void EnqueueBookingRequestedLandlordEmail(BookingRequestedLandlordPayload payload);
        void EnqueueBookingRequestedTenantEmail(BookingRequestedTenantPayload payload);

        void EnqueueBookingConfirmedTenantEmail(BookingConfirmedTenantPayload payload);
        void EnqueueBookingConfirmedLandlordEmail(BookingConfirmedLandlordPayload payload);

        void EnqueueBookingRejectedEmail(BookingRejectedPayload payload);

        void EnqueueBookingCancelledEmail(BookingCancelledPayload payload);

        void EnqueueBookingCompletedTenantEmail(BookingCompletedTenantPayload payload);
        void EnqueueBookingCompletedLandlordEmail(BookingCompletedLandlordPayload payload);
    }
}
