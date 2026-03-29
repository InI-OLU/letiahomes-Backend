using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class OtpEntry
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }            // FK to AppUser
        public string OtpHash { get; set; }           
        public string OtpSalt { get; set; }
        public OtpType Type { get; set; }             // Verification or PasswordReset
        public bool IsUsed { get; set; } = false;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser User { get; set; }
    }
}
