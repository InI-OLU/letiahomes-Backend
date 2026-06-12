using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.Features.Booking.Queries.TenantBookings;
using letiahomes.Application.RequestFeatures;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace letiahomes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingController (IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> TenantBookings([FromQuery] TenantBookingFilter filter, [FromQuery] Guid bookingId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new TenantBookingRequest(filter,userId,bookingId ), cancellationToken);
            if (result.IsFailure)
                return NotFound(result.Error);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Value.MetaData));
            return Ok(result);
        }
    }
}
