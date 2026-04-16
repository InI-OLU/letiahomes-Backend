using letiahomes.Domain.Common;
using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class Booking:AuditableEntity
    {
        public Guid PropertyId { get; set; }
        public Guid TenantProfileId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
        public long TotalAmountKobo { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // navigation
        public Property Property { get; set; }
        public TenantProfile Tenant { get; set; }
        public Payment? Payment { get; set; }
    }
}
