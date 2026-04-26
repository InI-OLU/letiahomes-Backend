using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Property
{
   public class PropertyResponse
    {
        public Guid Id { get; init; }
        public  string Title { get; set; }
        public string Description { get; set; }
        public  string Address { get; set; }
        public  string City { get; set; }
        public  string State { get; set; }
        public long PricePerNightKobo { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public PropertyType PropertyType { get; set; }
        public ListingType ListingType { get; init; }
        public bool IsAvailable { get; init; }
        public string? CoverImageUrl { get; init; }
        public ICollection<UnavailableDate> UnavailableDates { get; set; }
    }
}
