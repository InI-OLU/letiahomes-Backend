using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.CreateProperty
{
    public sealed class CreatePropertyCommandHandler:IRequestHandler<CreatePropertyCommand ,ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly UserManager<AppUser> _userManager;

        public CreatePropertyCommandHandler(IRepositoryManager repositoryManager,
                                            UserManager<AppUser> userManager) 
        {
            _repositoryManager = repositoryManager;
            _userManager = userManager;
        }

        public async Task<ApiResult<string>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            var landlord = await _repositoryManager.Landlords.FindAll(x => x.AppUserId == request.userId && x.IsVerified == true,false)
                                                              .FirstOrDefaultAsync();
            if (landlord == null)
            {
                return ApiResult<string>.Failure(new CustomError("404", "User not found or IsNotVerified"));
            }
            
            var Property = new Property
            {
                Title = request.request.Title,
                City = request.request.City,
                State = request.request.State,
                Description = request.request.Description,
                Address = request.request.Address,
                MaxGuests = request.request.MaxGuests,
                Bathrooms = request.request.Bathrooms,
                Bedrooms = request.request.Bedrooms,
                LandlordProfileId = landlord.Id,
                PropertyType = request.request.PropertyType,
                ListingType = request.request.ListingType,
                PricePerNightKobo = request.request.PricePerNightKobo,
                Images = [],
                Amenities = [],
                Bookings = [],
                UnavailableDates = [],
            };
              await _repositoryManager.Properties.AddAsync(Property);
            await _repositoryManager.SaveChangesAsync();
            //There should be a property Created count for admin to be able to track how many property are beind listed per time 
            //send an email using EmailService to The landlord confirming propertyCreation 
            //send a notification to the admin (adminDashboard) of a new propertyListing

            return ApiResult<string>.Success(
    $"Property {Property.Id} has been created successfully");
        }
    }
}
