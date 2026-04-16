using letiahomes.Domain.Common;
using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class Payment:AuditableEntity
    {
        public Guid BookingId { get; set; }
        public string PaystackReference { get; set; }   // from Paystack
        public long AmountKobo { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime PaidAt { get; set; }

        // navigation
        public Booking Booking { get; set; }
    }
}
