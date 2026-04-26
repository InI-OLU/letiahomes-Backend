using letiahomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.RequestFeatures
{
    public class PropertyFilterRequest:RequestParameters
    {
        public  string? Title { get; set; }
        public string? State { get; set; }
        public long? PricePerNightKobo { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public PropertyType? PropertyType { get; set; }
    }
}
