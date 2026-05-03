using letiahomes.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.SubmitPropertyForReview
{
    public record PropertyReviewCommand(Guid Propertyid, string UserId):IRequest<ApiResult<string>>;
    
}
