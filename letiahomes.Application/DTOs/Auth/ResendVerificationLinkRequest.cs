using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Auth
{
    public record ResendVerificationLinkRequest
    {
        public required string Email { get; init; }
    }
}
