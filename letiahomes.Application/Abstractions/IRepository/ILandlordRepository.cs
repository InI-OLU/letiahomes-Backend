using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.IRepository
{
    public interface ILandlordRepository : IBaseRepository<LandlordProfile>
    {
        Task<LandlordProfile?> GetLandlord(string UserId);
    }
}
