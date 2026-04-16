using letiahomes.Domain.Common;
using letiahomes.Domain.Entities;

public class LandlordProfile:AuditableEntity
{
    public string AppUserId { get; set; }

    public bool IsVerified { get; set; } = false;

    public ICollection<Property> Properties { get; set; } = [];
    public ICollection<Payout> Payouts { get; set; } = [];
    public AppUser AppUser { get; set; }
}