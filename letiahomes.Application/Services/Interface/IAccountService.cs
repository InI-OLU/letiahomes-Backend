using letiahomes.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Services.Interface
{
    public interface IAccountService
    {
        Task<AuthResponse> RegisterLandlord(RegisterLandlordRequest request);
    }
}
