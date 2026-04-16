using letiahomes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class RefreshToken:BaseEntity
    {
        public required string Token { get; set; } 
        public  required string UserId { get; set; } 
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
