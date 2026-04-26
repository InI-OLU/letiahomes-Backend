using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Property
{
    public class CreatePropertyRequest
    {
            public required string Title { get; set; }
            public required string Description { get; set; }
            public string Address { get; set; }
            public required string City { get; set; }
            public required string State { get; set; }
            public int MaxGuests { get; set; }
            public int Bedrooms { get; set; }
            public int Bathrooms { get; set; }
            public PropertyType PropertyType { get; set; }
            public ListingType ListingType { get; set; }
            public long PricePerNightKobo { get; set; }
    }
}
