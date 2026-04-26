using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.RequestFeatures;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Infrastructure.Repository
{
    public class PropertyRepository(ApplicationDbContext context) : BaseRepository<Property>(context), IPropertyRepository
    {
        private readonly ApplicationDbContext _dbContext = context;

       public async Task<PagedList<Property>> GetAllProperties(RequestParameters parameters)
        {
            var query =  _dbContext.Properties.AsQueryable();
            query = query.Where(x => x.IsApproved != false && x.IsAvailable != false)
                         .AsNoTracking();
            return await PagedList<Property>.ToPagedList(query,parameters.pageNumber,parameters.pageSize);
        }
        public async Task<PagedList<Property>> FilterBy(PropertyFilterRequest request)
        {
            var query = _dbContext.Properties.AsQueryable();
            query = query.Where(x => x.IsApproved != false && x.IsAvailable != false);
            if (request.Bathrooms != null)
                query = query.Where(x => x.Bathrooms == request.Bathrooms);
            if (request.Bedrooms != null)
                query = query.Where(x => x.Bedrooms == request.Bedrooms);
            if (string.IsNullOrEmpty(request.Title))
                query = query.Where(x => x.Title == request.Title);
            if (string.IsNullOrEmpty(request.State))
                query = query.Where(x => x.State == request.State);
            if (request.PricePerNightKobo == request.PricePerNightKobo)
                query = query.Where(x => x.PricePerNightKobo <= request.PricePerNightKobo);
            if (request.PropertyType != default)
                query = query.Where(x => x.PropertyType == request.PropertyType);
            if (request.ValidDateRange)
                query = query.Where(x => x.CreatedAt >= request.StartDate && x.CreatedAt <= request.EndDate);
            if (!string.IsNullOrWhiteSpace(request.SearchBy))
                query = query.Where(x => x.Title.Contains(request.SearchBy) || x.Address.Contains(request.SearchBy));
            query = query.AsNoTracking()
                         .OrderBy(x => x.Title)
                          .ThenByDescending(x => x.CreatedAt);
            return await PagedList<Property>.ToPagedList(query, request.pageNumber, request.pageSize);











        }
    }
}
