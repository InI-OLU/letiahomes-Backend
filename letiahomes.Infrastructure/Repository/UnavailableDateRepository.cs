using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Infrastructure.Repository
{
    public class UnavailableDateRepository(ApplicationDbContext context) : BaseRepository<UnavailableDate>(context), IUnavailableDateRepository
    {
        private readonly ApplicationDbContext context = context;
    }
}
