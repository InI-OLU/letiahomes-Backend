using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
