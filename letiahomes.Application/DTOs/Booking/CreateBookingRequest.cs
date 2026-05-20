using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Booking
{
    public sealed record CreateBookingRequest
    {
        public required Guid PropertyId { get; init; }
        public required DateTime CheckIn { get; init; }
        public required DateTime CheckOut { get; init; }
        public required int NumberOfGuests { get; init; }
    }
}
