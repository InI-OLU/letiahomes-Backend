using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Property
{
    public sealed record CreatePropertyAmenityRequest
    {
        public required Guid PropertyId { get; init; }
        public required string Name { get; init; }  // WiFi, Pool, AC, Generator etc
    }
}
