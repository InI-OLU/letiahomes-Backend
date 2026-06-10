using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.RequestFeatures;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Queries.TenantBookings
{
    public class TenantBookingHandler(IRepositoryManager repositoryManager) : IRequestHandler<TenantBookingRequest, ApiResult<PagedList<BookingResponse>>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public async  Task<ApiResult<PagedList<BookingResponse>>> Handle(TenantBookingRequest request, CancellationToken cancellationToken)
        {
            var tenant = await _repositoryManager.Tenants.Get(t => t.AppUser.IsActive == true);
            throw new NotImplementedException();
        }
    }
}
