using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Booking
{
    public sealed class AdminBookingResponse
    {
        public Guid Id { get; init; }
        public string PropertyTitle { get; init; } = string.Empty;
        public string TenantFullName { get; init; } = string.Empty;
        public string TenantEmail { get; init; } = string.Empty;
        public string LandlordFullName { get; init; } = string.Empty;
        public string LandlordEmail { get; init; } = string.Empty;
        public DateTime CheckIn { get; init; }
        public DateTime CheckOut { get; init; }
        public int NightsCount { get; init; }
        public long SubtotalKobo { get; init; }
        public long PlatformFeeKobo { get; init; }
        public long TotalAmountKobo { get; init; }
        public string Status { get; init; } = string.Empty;
        public string? CancellationReason { get; init; }
        public string? RejectionReason { get; init; }
        public DateTime? CancelledAt { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime ExpiresAt { get; init; }
    }
}
