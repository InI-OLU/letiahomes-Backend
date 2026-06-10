namespace letiahomes.Application.DTOs.Booking
{
    public sealed class BookingResponse
    {
        public Guid Id { get; init; }
        public string PropertyTitle { get; init; } = string.Empty;
        public string? CoverImageUrl { get; init; }
        public DateTime CheckIn { get; init; }
        public DateTime CheckOut { get; init; }
        public int NightsCount { get; init; }
        public int NumberOfGuests { get; init; }
        public long SubtotalKobo { get; init; }
        public long TotalAmountKobo { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime ExpiresAt { get; init; }
    }
}
