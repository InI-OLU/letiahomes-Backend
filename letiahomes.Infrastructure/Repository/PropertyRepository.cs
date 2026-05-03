using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.RequestFeatures;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Infrastructure.Repository
{
    public class PropertyRepository(ApplicationDbContext context) : BaseRepository<Property>(context), IPropertyRepository
    {
        private readonly ApplicationDbContext _dbContext = context;

        public async Task<PagedList<Property>> GetAllProperties(RequestParameters parameters)
        {
            var query = _dbContext.Properties.AsQueryable();
            query = query.Where(x => x.IsApproved != false && x.IsAvailable != false)
                         .Include(x => x.Images)
                         .Include(x => x.UnavailableDates)
                         .AsNoTracking();
            return await PagedList<Property>.ToPagedList(query, parameters.pageNumber, parameters.pageSize);
        }
        public async Task<PagedList<Property>> FilterBy(PropertyFilterRequest request)
        {
            var query = _dbContext.Properties.AsQueryable();
            query = query.Where(x => x.IsApproved != false && x.IsAvailable != false);
            if (request.Bathrooms.HasValue)
                query = query.Where(x => x.Bathrooms == request.Bathrooms);
            if (request.Bedrooms.HasValue)
                query = query.Where(x => x.Bedrooms == request.Bedrooms);
            if (!string.IsNullOrEmpty(request.Title))
                query = query.Where(x => x.Title == request.Title);
            if (!string.IsNullOrEmpty(request.State))
                query = query.Where(x => x.State == request.State);
            if (request.PricePerNightKobo.HasValue)
                query = query.Where(x => x.PricePerNightKobo <= request.PricePerNightKobo);
            if (request.PropertyType != default)
                query = query.Where(x => x.PropertyType == request.PropertyType);
            query = query.AsNoTracking()
                         .OrderBy(x => x.Title)
                          .ThenByDescending(x => x.CreatedAt);
          //  System.Diagnostics.Debug.WriteLine(query.ToQueryString());
            return await PagedList<Property>.ToPagedList(query, request.pageNumber, request.pageSize);
        }
        public async Task<PagedList<Property>> GetFeaturedProperty(RequestParameters parameters)
        {
            var query = _dbContext.Properties.AsQueryable();
            query = query.Where(x => x.IsAvailable == true && x.IsApproved == true)
                         .OrderByDescending(x => x.CreatedAt)
                         .Take(5)
                         .AsNoTracking();
            return await PagedList<Property>.ToPagedList(query, parameters.pageNumber, parameters.pageSize);
        }
        public async Task<PropertyResponse?> GetPropertyWithAmenityAndImage(Guid PropertyId)
        {
            var result = await _dbContext.Properties.AsQueryable()
                                                    .Where(x => x.Id == PropertyId)
                                                    .Select(property => new PropertyResponse
                                                    {
                                                        Id = property.Id,
                                                        Title = property.Title,
                                                        State = property.State,
                                                        City = property.City,
                                                        Description = property.Description,
                                                        Address = property.Address,
                                                        PricePerNightKobo = property.PricePerNightKobo,
                                                        MaxGuests = property.MaxGuests,
                                                        Bedrooms = property.Bedrooms,
                                                        Bathrooms = property.Bathrooms,
                                                        PropertyType = property.PropertyType,
                                                        ListingType = property.ListingType,
                                                        IsAvailable = property.IsAvailable,
                                                        IsApproved = property.IsApproved,
                                                        UnavailableDates = property.UnavailableDates,
                                                        Images = property.Images
                                                    }).FirstOrDefaultAsync();
            return  result;

        }
       
    }
}
