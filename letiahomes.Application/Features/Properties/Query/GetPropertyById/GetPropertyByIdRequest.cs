using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.RequestFeatures;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Query.GetPropertyById
{
    public record GetPropertyByIdRequest(Guid PropertyId):IRequest<ApiResult<PropertyResponse>>;
    
}
