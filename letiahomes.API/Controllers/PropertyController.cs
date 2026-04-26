using letiahomes.Application.DTOs.Property;
using letiahomes.Application.Features.Properties.Command.CreateProperty;
using letiahomes.Application.Features.Properties.Command.CreatePropertyAmenity;
using letiahomes.Application.Features.Properties.Command.UploadPropertyPicture;
using letiahomes.Application.Features.Properties.Query.FilterProperty;
using letiahomes.Application.Features.Properties.Query.GetAllProperty;
using letiahomes.Application.RequestFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace letiahomes.API.Controllers
{
    [Route("api/property")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PropertyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize(Roles = "Admin,Landlord")]
        [HttpPost("create-property")]
        public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequest request,
                                                              CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(
        new CreatePropertyCommand(request,userId), cancellationToken);
            return Ok(result);
        }
        [Authorize(Roles = "Admin,Landlord")]
        [HttpPost("upload-property-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPropertyImage([FromForm] UploadMultiplePropertyPicture uploadRequest,
                                                               CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
        new UploadPropertyPictureCommand(uploadRequest), cancellationToken);
            return result.IsSuccess ? Ok(result) : Conflict(result);
        }

        [Authorize(Roles = "Admin,Landlord")]
        [HttpPost("create-property-amenities")]
        public async Task<IActionResult> CreatePropertyAmenity([FromBody] CreatePropertyAmenityRequest propertyAmenityRequest,
                                                                CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new CreatePropertyAmenityCommand(propertyAmenityRequest,userId),cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Landlord,Tenant")]
        [HttpGet("properties")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllProperties([FromQuery] RequestParameters parameters, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllPropertiesRequest ( parameters ),cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Value.MetaData));

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Landlord,Tenant")]
        [HttpGet("filter-properties")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FilterProperties([FromQuery] PropertyFilterRequest parameters, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new FilterPropertiesRequest(parameters), cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Value.MetaData));

            return Ok(result);
        }
    }
}
