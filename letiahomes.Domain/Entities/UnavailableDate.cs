using letiahomes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class UnavailableDate:BaseEntity
    {
        public  Guid PropertyId { get; set; }
        public DateTime Date { get; set; }  // each blocked date is one record
        public Guid? BookingId { get; set; }
        public  Property? Property { get; set; }
        public Booking? Booking { get; set; }

    }
}
