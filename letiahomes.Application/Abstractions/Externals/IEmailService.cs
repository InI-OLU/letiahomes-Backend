using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.Externals
{
    public interface IEmailService
    {
        Task SendAccountVerificationAsync(string recipient, string firstName, string link);
        Task SendAccountVerifiedAsync(string recipient, string firstName, string loginLink);
        Task SendBookingCancelledEmailAsync(BookingCancelledPayload payload);
        Task SendBookingCompletedLandlordEmailAsync(BookingCompletedLandlordPayload payload);
        Task SendBookingCompletedTenantEmailAsync(BookingCompletedTenantPayload payload);
        Task SendBookingConfirmedLandlordEmailAsync(BookingConfirmedLandlordPayload payload);
        Task SendBookingConfirmedTenantEmailAsync(BookingConfirmedTenantPayload payload);
        Task SendBookingRejectedEmailAsync(BookingRejectedPayload payload);
        Task SendBookingRequestedLandlordEmailAsync(BookingRequestedLandlordPayload payload);
        Task SendBookingRequestedTenantEmailAsync(BookingRequestedTenantPayload payload);
        Task SendPasswordResetAsync(string recipient, string firstName, string resetLink);
    }
}
