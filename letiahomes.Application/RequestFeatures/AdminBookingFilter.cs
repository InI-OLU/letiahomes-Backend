using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.RequestFeatures
{
    public class AdminBookingFilter:RequestParameters
    {
        public string? Title { get; set; }
        public BookingStatus? BookingStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
