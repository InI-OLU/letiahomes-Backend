using letiahomes.Domain.Common;
using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class Property:AuditableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; } = "Nigeria";
        public long PricePerNightKobo { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsApproved { get; set; } = false;  // admin approves listing
        public Guid LandlordProfileId { get; set; }
        public PropertyType PropertyType { get; set; }      
        public ListingType ListingType { get; set; }
        // navigation
        public LandlordProfile Landlord { get; set; }
        public ICollection<PropertyImage> Images { get; set; }
        public ICollection<PropertyAmenity> Amenities { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<UnavailableDate> UnavailableDates { get; set; }
    }
}
