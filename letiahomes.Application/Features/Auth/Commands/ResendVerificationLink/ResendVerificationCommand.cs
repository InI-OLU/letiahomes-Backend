using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Auth.Commands.ResendVerificationLink
{
    public  record ResendVerificationCommand(ResendVerificationLinkRequest request)
    : IRequest<ApiResult<string>>;
}
