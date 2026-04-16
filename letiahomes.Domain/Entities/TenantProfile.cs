using letiahomes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class TenantProfile:AuditableEntity
    {
        public string AppUserId { get; set; }         // FK to AppUser

        public AppUser AppUser { get; set; }          // navigation back to user
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
