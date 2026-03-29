using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Entities
{
    public class UnavailableDate
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime Date { get; set; }  // each blocked date is one record
        public Property Property { get; set; }
    }
}
