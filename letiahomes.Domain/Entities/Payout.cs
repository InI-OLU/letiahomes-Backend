using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class Payout
    {
        public Guid Id { get; set; }
        public Guid LandlordProfileId { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }             // TotalAmount minus platform fee
        public decimal PlatformFee { get; set; }        // your cut
        public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation
        public LandlordProfile Landlord { get; set; }
    }
}
