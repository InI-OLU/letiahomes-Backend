using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class Property
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; } = "Nigeria";
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsApproved { get; set; } = false;  // admin approves listing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid LandlordProfileId { get; set; }

        // navigation
        public LandlordProfile Landlord { get; set; }
        public ICollection<PropertyImage> Images { get; set; }
        public ICollection<PropertyAmenity> Amenities { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<UnavailableDate> UnavailableDates { get; set; }
    }
}
