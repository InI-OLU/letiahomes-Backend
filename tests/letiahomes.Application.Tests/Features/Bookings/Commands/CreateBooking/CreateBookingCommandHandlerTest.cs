using letiahomes.Domain.Entities;
using MockQueryable;
using MockQueryable.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Tests.Features.Booking.Commands.CreateBooking
{
    public class CreateBookingCommandHandlerTest
    {
        [Fact]
        public async Task Handle_MoreThanThreeActiveBooking_ReturnsFailure()
        {
            TenantProfile tenants =
                new TenantProfile
                {
                    AppUserId = "1234",
                    Bookings = [
                        new {}
                        ]
                };

        }
    }
}
