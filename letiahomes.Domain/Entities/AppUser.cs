using Microsoft.AspNetCore.Identity;

namespace letiahomes.Domain.Entities
{
    public class AppUser:IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
