using letiahomes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class PropertyAmenity:AuditableEntity
    {
        public Guid PropertyId { get; set; }
        public string Name { get; set; }  // WiFi, Pool, AC, Generator etc
        public Property Property { get; set; }
    }
}
