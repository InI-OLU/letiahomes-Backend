using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Auth
{
    public class RevokeTokenRequest
    {
        public required string RefreshToken { get; init; }
        public required string UserId { get; init; }
    }
}
