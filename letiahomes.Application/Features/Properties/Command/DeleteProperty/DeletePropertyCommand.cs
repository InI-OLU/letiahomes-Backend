using letiahomes.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.DeleteProperty
{
    public sealed record DeletePropertyCommand(Guid PropertyId,string userId):IRequest<ApiResult<string>>;
    
}
