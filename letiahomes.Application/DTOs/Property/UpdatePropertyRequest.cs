using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Property
{
   public record UpdatePropertyRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public long PricePerNightKobo { get; set; }
    }
}
