using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.RequestFeatures;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Query.FilterProperty
{
    public sealed record FilterPropertiesRequest(PropertyFilterRequest request):IRequest<ApiResult<PagedList<PropertyResponse>>>;
    
}
