

namespace letiahomes.Domain.Enums
{
    public enum BookingStatus
    {
        Pending,        // created, waiting for payment
        Confirmed,      // payment successful
        Cancelled,      // tenant or landlord cancelled
        Completed,      // checkout date passed
        Rejected        // landlord rejected
    }
}
