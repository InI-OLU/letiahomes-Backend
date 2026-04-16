using letiahomes.Domain.Common;
using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class Payout:AuditableEntity
    {
        public Guid LandlordProfileId { get; set; }
        public Guid BookingId { get; set; }
        public long AmountKobo { get; set; }
        public long PlatformFeeKobo { get; set; }       // your cut
        public PayoutStatus Status { get; set; } = PayoutStatus.Pending;

        // navigation
        public LandlordProfile Landlord { get; set; }
    }
}
