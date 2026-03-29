using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class LandlordProfile
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; }         
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public string GovernmentId { get; set; }
        public string? Address { get; set; }
        public bool IsVerified { get; set; } = false; // admin verifies landlord separately
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser AppUser { get; set; }          // navigation back to user
        public ICollection<Property> Properties { get; set; } = [];
    }
}
