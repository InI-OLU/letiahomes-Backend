using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.Features.Booking.Commands.CancelBooking;
using letiahomes.Application.Features.Booking.Commands.ConfirmBooking;
using letiahomes.Application.Features.Booking.Commands.CreateBooking;
using letiahomes.Application.Features.Booking.Commands.LandlordCancelBooking;
using letiahomes.Application.Features.Booking.Commands.RejectBooking;
using letiahomes.Application.Features.Booking.Queries.AdminBookings;
using letiahomes.Application.Features.Booking.Queries.LanlordBookings;
using letiahomes.Application.Features.Booking.Queries.TenantBookings;
using letiahomes.Application.RequestFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace letiahomes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // POST /api/booking
        [HttpPost]
        public async Task<ActionResult<ApiResult<string>>> CreateBooking(
            [FromBody] CreateBookingRequest request,
            CancellationToken cancellationToken)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
                new CreateBookingCommand(request, UserId!), cancellationToken);

            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }

        // POST /api/booking/{bookingId}/confirm
        [HttpPost("{bookingId:guid}/confirm")]
        public async Task<ActionResult<ApiResult<string>>> ConfirmBooking(
            Guid bookingId,
            CancellationToken cancellationToken)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
                new ConfirmBookingCommand(bookingId, UserId!), cancellationToken);

            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }

        // POST /api/booking/{bookingId}/reject
        [HttpPost("{bookingId:guid}/reject")]
        public async Task<ActionResult<ApiResult<string>>> RejectBooking(
            Guid bookingId,
            [FromBody] RejectBookingRequest request,
            CancellationToken cancellationToken)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
                new RejectBookingCommand(bookingId, UserId!, request.Reason), cancellationToken);

            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }
        // POST /api/booking/{bookingId}/tenant-cancel
        [HttpPost("{bookingId:guid}/tenant-cancel")]
        public async Task<ActionResult<ApiResult<string>>> TenantCancelBooking(
            Guid bookingId,
            [FromBody] CancelBookingRequest request,
            CancellationToken cancellationToken)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
                new TenantCancelBookingCommand(bookingId, UserId!, request.Reason), cancellationToken);

            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }

        // POST /api/booking/{bookingId}/landlord-cancel
        [HttpPost("{bookingId:guid}/landlord-cancel")]
        public async Task<ActionResult<ApiResult<string>>> LandlordCancelBooking(
            Guid bookingId,
            [FromBody] CancelBookingRequest request,
            CancellationToken cancellationToken)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
                new LandlordCancelBookingCommand(bookingId, UserId!, request.Reason), cancellationToken);

            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }

        // GET /api/booking/my-bookings
        [HttpGet("my-bookings")]
        public async Task<ActionResult<ApiResult<PagedList<BookingResponse>>>> TenantBookings(
            [FromQuery] TenantBookingFilter filter,
            Guid BookingId,
            CancellationToken cancellationToken)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
                new TenantBookingRequest(filter, UserId!,BookingId), cancellationToken);

            if (result.IsFailure)
                return NotFound(result);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Value!.MetaData));
            return Ok(result);
        }


        // GET /api/booking/property-bookings
        [HttpGet("property-bookings")]
        public async Task<ActionResult<ApiResult<PagedList<LandlordBookingResponse>>>> LandlordBookings(
            [FromQuery] LandlordBookingFilter filter,
            CancellationToken cancellationToken)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
                new LandlordBookingRequest(UserId!,filter), cancellationToken);

            if (result.IsFailure)
                return NotFound(result);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Value!.MetaData));
            return Ok(result);
        }

        // GET /api/booking/admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult<PagedList<AdminBookingResponse>>>> AdminBookings(
            [FromQuery] AdminBookingFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new AdminBookingRequest(filter), cancellationToken);

            if (result.IsFailure)
                return NotFound(result);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Value!.MetaData));
            return Ok(result);
        }
    }
}