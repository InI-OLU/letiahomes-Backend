using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Auth.Commands.RevokeToken
{
    public sealed record RevokeTokenCommand(string UserId)
        : IRequest<ApiResult<string>>;
}
