using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Enums
{
    public enum PayoutStatus
    {
        Pending,        // payment received, payout not sent yet
        Processing,     // being sent to landlord
        Completed,      // landlord received money
        Failed
    }
}
