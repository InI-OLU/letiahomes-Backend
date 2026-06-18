using Hangfire;
using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.DTOs.Notification;

namespace letiahomes.Infrastructure.ExternalServices;

public sealed class NotificationService : INotificationService
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

    public void EnqueueBookingRequestedLandlordEmail(BookingRequestedLandlordPayload payload)
        => _backgroundJobClient.Enqueue<NotificationJobService>(
            job => job.ProcessBookingRequestedLandlordAsync(payload));

    public void EnqueueBookingRequestedTenantEmail(BookingRequestedTenantPayload payload)
        => _backgroundJobClient.Enqueue<NotificationJobService>(
            job => job.ProcessBookingRequestedTenantAsync(payload));

    public void EnqueueBookingConfirmedTenantEmail(BookingConfirmedTenantPayload payload)
        => _backgroundJobClient.Enqueue<NotificationJobService>(
            job => job.ProcessBookingConfirmedTenantAsync(payload));

    public void EnqueueBookingConfirmedLandlordEmail(BookingConfirmedLandlordPayload payload)
        => _backgroundJobClient.Enqueue<NotificationJobService>(
            job => job.ProcessBookingConfirmedLandlordAsync(payload));

    public void EnqueueBookingRejectedEmail(BookingRejectedPayload payload)
        => _backgroundJobClient.Enqueue<NotificationJobService>(
            job => job.ProcessBookingRejectedAsync(payload));

    public void EnqueueBookingCancelledEmail(BookingCancelledPayload payload)
        => _backgroundJobClient.Enqueue<NotificationJobService>(
            job => job.ProcessBookingCancelledAsync(payload));

    public void EnqueueBookingCompletedTenantEmail(BookingCompletedTenantPayload payload)
        => _backgroundJobClient.Enqueue<NotificationJobService>(
            job => job.ProcessBookingCompletedTenantAsync(payload));

    public void EnqueueBookingCompletedLandlordEmail(BookingCompletedLandlordPayload payload)
        => _backgroundJobClient.Enqueue<NotificationJobService>(
            job => job.ProcessBookingCompletedLandlordAsync(payload));
}