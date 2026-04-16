using letiahomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.IRepository
{
    public interface ITenantRepository:IBaseRepository<TenantProfile>
    {
    }
}
