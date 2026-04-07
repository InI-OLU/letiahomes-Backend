using letiahomes.Domain.Entities;

public class LandlordProfile
{
    public Guid Id { get; set; }
    public string AppUserId { get; set; }

    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Property> Properties { get; set; } = [];
    public ICollection<Payout> Payouts { get; set; } = [];
    public AppUser AppUser { get; set; }
}