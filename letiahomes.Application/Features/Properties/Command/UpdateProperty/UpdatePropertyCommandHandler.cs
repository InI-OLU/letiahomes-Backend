using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.UpdateProperty
{
    public class UpdatePropertyCommandHandler(IRepositoryManager repositoryManager,ILogger<UpdatePropertyCommandHandler> logger) : IRequestHandler<UpdatePropertyCommand, ApiResult<PropertyResponse>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;
        private readonly ILogger<UpdatePropertyCommandHandler> _logger = logger;

        public async Task<ApiResult<PropertyResponse>> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var propertyExists =  _repositoryManager.Properties
                                                            .FindAll(x => x.Id == request.request.PropertyId && x.Landlord.AppUserId == request.userId , true)
                                                            .FirstOrDefaultAsync(cancellationToken);
            var property = new Property
            {
                Id = request.request.PropertyId
            };
            _repositoryManager.Properties.Attach(property);

            var entry = _repositoryManager.Properties.Entry(property);
            if (propertyExists is null)
                return ApiResult<PropertyResponse>.Failure(new CustomError("404", "You are not permitted to make this change"));
          
            if (request.request.Title is not null)
            {
                property.Title = request.request.Title;
                entry.Property(x => x.Title).IsTemporary = true;
            }
              
            

            if (request.request.Description is not null)
            {
                property.Description = request.request.Description;
                entry.Property(x => x.Description).IsModified = true;
            }


            if (request.request.IsAvailable.HasValue)
            {
                property.IsAvailable = request.request.IsAvailable.Value;
                entry.Property(x => x.IsAvailable).IsModified = true;
            }

            if (request.request.PricePerNightKobo.HasValue)
            {
                property.PricePerNightKobo = request.request.PricePerNightKobo.Value;
                entry.Property(x => x.PricePerNightKobo).IsModified = true;

                // Reset approval on price change
                property.IsApproved = false;
                entry.Property(x => x.IsApproved).IsModified = true;
            }

            property.UpdatedAt = DateTime.UtcNow;
            entry.Property(x => x.UpdatedAt).IsModified = true;
            await _repositoryManager.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Property with Id: {PropertyId} updated at {Timestamp}",
                                     property.Id,
                                     property.UpdatedAt
                                  );
            return ApiResult<PropertyResponse>.Success(new PropertyResponse
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                IsAvailable = property.IsAvailable,
                PricePerNightKobo = property.PricePerNightKobo,
            });

        }
    }
}
