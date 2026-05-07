using letiahomes.Domain.Enums;

namespace letiahomes.Application.DTOs.Property;

public sealed record PropertyResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public long PricePerNightKobo { get; init; }
    public int MaxGuests { get; init; }
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public PropertyType PropertyType { get; init; }
    public ListingType ListingType { get; init; }
    public bool IsAvailable { get; init; }
    public bool IsApproved { get; init; }
    public IReadOnlyList<UnavailableDateResponse> UnavailableDates { get; init; } = [];
    public IReadOnlyList<PropertyImageResponse> Images { get; init; } = [];
}