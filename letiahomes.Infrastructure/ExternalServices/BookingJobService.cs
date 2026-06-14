using Hangfire;
using letiahomes.Application.Abstractions.Jobs;

namespace letiahomes.Infrastructure.ExternalServices
{
    public class BookingJobService
    {
        public static class AwaitingPaymentExpiryJobScheduler
        {
            public static void RegisterRecurringJobs()
            {
                RecurringJob.AddOrUpdate<IBookingAutoExpiryJob>(
                    "expire-pending-bookings",
                    job => job.ExpirePendingBookingsAsync(),
                    "*/15 * * * *");

                RecurringJob.AddOrUpdate<ICheckoutJob>(
                   "Checkout-expired-bookings",
                   job => job.MarkAsCheckOutJob(),
                   "*/15 * * * *");

                RecurringJob.AddOrUpdate<IAwaitingPaymentExpiryJob>(
                    "expire-unpaid-bookings",
                    job => job.ExpireUnpaidBookingsAsync(),
                    "*/5 * * * *");
            }
        }
    }
}
 