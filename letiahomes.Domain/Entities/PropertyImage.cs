using letiahomes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class PropertyImage:AuditableEntity
    {
        public Guid PropertyId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public bool IsCoverImage { get; set; } = false;
        public Property Property { get; set; }
    }
}
