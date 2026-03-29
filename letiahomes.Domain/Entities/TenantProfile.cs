using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class TenantProfile
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; }         // FK to AppUser
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser AppUser { get; set; }          // navigation back to user
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
