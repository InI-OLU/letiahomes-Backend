using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.RequestFeatures;
using MediatR;

namespace letiahomes.Application.Features.Properties.Query.GetFeaturedProperty
{
    public sealed record GetFeaturedPropertyRequest(RequestParameters request) : IRequest<ApiResult<PagedList<PropertyResponse>>>;
    
}
