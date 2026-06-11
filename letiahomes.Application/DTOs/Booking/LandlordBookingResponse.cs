using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Booking
{
    public sealed class LandlordBookingResponse
    {
        public Guid Id { get; init; }
        public string PropertyTitle { get; init; } = string.Empty;
        public string TenantFirstName { get; init; } = string.Empty;
        public DateTime CheckIn { get; init; }
        public DateTime CheckOut { get; init; }
        public int NightsCount { get; init; }
        public long TotalAmountKobo { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

}
