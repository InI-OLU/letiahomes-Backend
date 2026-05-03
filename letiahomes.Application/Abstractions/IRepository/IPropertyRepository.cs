using letiahomes.Application.DTOs.Property;
using letiahomes.Application.RequestFeatures;
using letiahomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.IRepository
{
    public interface IPropertyRepository : IBaseRepository<Property>
    {
        Task<PagedList<Property>> FilterBy(PropertyFilterRequest request);
        Task<PagedList<Property>> GetAllProperties(RequestParameters parameters);
        Task<PagedList<Property>> GetFeaturedProperty(RequestParameters parameters);
        Task<PropertyResponse?> GetPropertyWithAmenityAndImage(Guid PropertyId);
    }
}
