using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.Features.Booking.Commands.CreateBooking;
using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using Moq;


namespace letiahomes.Application.Tests.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandlerTest
    {
        [Fact]
        public async Task Handle_MoreThanThreeActiveBooking_ReturnsFailure()
        {
            TenantProfile tenant =
                new TenantProfile
                {
                    Id = Guid.NewGuid(),
                    AppUserId = "1234",
                    AppUser = new AppUser {
                        Email = "tenant@example.com",
                        FirstName = "Jane",
                        LastName = "Doe",
                        IsActive = true,
                        IsVerified = true
                    },
                    Bookings = new List<Booking>
                    {
                          new Booking { Status = BookingStatus.Pending },
                          new Booking { Status = BookingStatus.Pending },
                          new Booking { Status = BookingStatus.Pending }
                    }
                };

               var propertyId = Guid.NewGuid();
               var landlordProfileId = Guid.NewGuid();
               var property = new Property
               {
               Id = propertyId,
               Title = "Test Apartment",
               IsAvailable = true,
               IsApproved = true,
               LandlordProfileId = landlordProfileId,
               PricePerNightKobo = 50000
               };

            var landlord = new LandlordProfile
            {
                Id = landlordProfileId,
                AppUserId = "landlord-1"
            };
            var tenantRepoMock = new Mock<ITenantRepository>();
            tenantRepoMock
                .Setup(r => r.GetTenant(It.IsAny<string>()))
                .ReturnsAsync(tenant);
            var propertyRepoMock = new Mock<IPropertyRepository>();
            propertyRepoMock
                .Setup(r => r.GetByIdAsync(propertyId))
                .ReturnsAsync(property);
            var landlordRepoMock = new Mock<ILandlordRepository>();
            landlordRepoMock
                .Setup(r => r.GetByIdAsync(landlordProfileId))
                .ReturnsAsync(landlord);

            var repositoryManagerMock = new Mock<IRepositoryManager>();
            repositoryManagerMock
                .Setup(r => r.Tenants)
                .Returns(tenantRepoMock.Object);
            repositoryManagerMock.Setup(r => r.Properties).Returns(propertyRepoMock.Object);
            repositoryManagerMock.Setup(r => r.Landlords).Returns(landlordRepoMock.Object);

            var notificationServiceMock = new Mock<INotificationService>();
            var handler = new CreateBookingCommandHandler(
                repositoryManagerMock.Object,
                notificationServiceMock.Object);

            var command = new CreateBookingCommand(
            new CreateBookingRequest
            {
                 PropertyId = Guid.NewGuid(),
                  CheckIn = DateTime.UtcNow.AddDays(2),
                 CheckOut = DateTime.UtcNow.AddDays(5),
                  NumberOfGuests = 2
            },
            "1234");

            var result = await handler.Handle(command, CancellationToken.None);
            Assert.False(result.IsSuccess);
            Assert.Equal("400", result.Error!.code);
            Assert.Equal("You cannot have more than 3 pending bookings at a time", result.Error!.message);
        }
        [Fact]
        public async Task Handle_CheckInIsToday_ReturnsFailure()
        {
            var repositoryManagerMock = new Mock<IRepositoryManager>();
            var notificationServiceMock = new Mock<INotificationService>();
            var handler = new CreateBookingCommandHandler(
                repositoryManagerMock.Object,
                notificationServiceMock.Object);

            var command = new CreateBookingCommand(
                new CreateBookingRequest
                {
                    PropertyId = Guid.NewGuid(),
                    CheckIn = DateTime.UtcNow,       
                    CheckOut = DateTime.UtcNow.AddDays(3),
                    NumberOfGuests = 2
                },
                "1234");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("400", result.Error!.code);
            Assert.Equal("Check-in must be at least 1 day in the future", result.Error!.message);
        }

        [Fact]
        public async Task Handle_CheckOutBeforeCheckIn_ReturnsFailure()
        {
            var repositoryManagerMock = new Mock<IRepositoryManager>();
            var notificationServiceMock = new Mock<INotificationService>();
            var handler = new CreateBookingCommandHandler(
                repositoryManagerMock.Object,
                notificationServiceMock.Object);

            var command = new CreateBookingCommand(
                new CreateBookingRequest
                {
                    PropertyId = Guid.NewGuid(),
                    CheckIn = DateTime.UtcNow.AddDays(5),
                    CheckOut = DateTime.UtcNow.AddDays(2),   
                    NumberOfGuests = 2
                },
                "1234");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("400", result.Error!.code);
            Assert.Equal("Check-out must be after check-in", result.Error!.message);
        }
        [Fact]
        public async Task Handle_NightsGreaterThanNinetyDays_ReturnsFailure()
        {
            var repositoryManagerMock = new Mock<IRepositoryManager>();
            var notificationServiceMock = new Mock<INotificationService>();
            var handler = new CreateBookingCommandHandler(
                repositoryManagerMock.Object,
                notificationServiceMock.Object);

            var command = new CreateBookingCommand(
                new CreateBookingRequest
                {
                    PropertyId = Guid.NewGuid(),
                    CheckIn = DateTime.UtcNow.AddDays(1),
                    CheckOut = DateTime.UtcNow.AddDays(95),   
                    NumberOfGuests = 2
                },
                "1234");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("400", result.Error!.code);
            Assert.Equal("Maximum booking duration is 90 nights", result.Error!.message);
        }
        [Fact]
        public async Task Handle_TenantNotFound_ReturnsFailure()
        {
            var repositoryManagerMock = new Mock<IRepositoryManager>();
            var tenantRepoMock = new Mock<ITenantRepository>();
            tenantRepoMock
                .Setup(r => r.GetTenant(It.IsAny<string>()))
                .ReturnsAsync((TenantProfile?)null);
            repositoryManagerMock.Setup(r => r.Tenants).Returns(tenantRepoMock.Object);

            var notificationServiceMock = new Mock<INotificationService>();
            var handler = new CreateBookingCommandHandler(
                repositoryManagerMock.Object, notificationServiceMock.Object);

            var command = new CreateBookingCommand(
                new CreateBookingRequest
                {
                    PropertyId = Guid.NewGuid(),
                    CheckIn = DateTime.UtcNow.AddDays(2),
                    CheckOut = DateTime.UtcNow.AddDays(5),
                    NumberOfGuests = 2
                },
                "1234");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("400", result.Error!.code);
            Assert.Equal("User not Permitted to make booking", result.Error!.message);
        }

        [Fact]
        public async Task Handle_PropertyNotAvailable_ReturnsFailure()
        {
            var tenant = new TenantProfile
            {
                Id = Guid.NewGuid(),
                AppUser = new AppUser
                {
                    Email = "tenant@example.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    IsActive = true,
                    IsVerified = true
                },
                Bookings = new List<Booking>()
            };

            var tenantRepoMock = new Mock<ITenantRepository>();
            tenantRepoMock.Setup(r => r.GetTenant(It.IsAny<string>())).ReturnsAsync(tenant);

            var propertyId = Guid.NewGuid();
            var propertyRepoMock = new Mock<IPropertyRepository>();
            propertyRepoMock
                .Setup(r => r.GetByIdAsync(propertyId))
                .ReturnsAsync((Property?)null);   // not found — same branch as IsAvailable == false

            var repositoryManagerMock = new Mock<IRepositoryManager>();
            repositoryManagerMock.Setup(r => r.Tenants).Returns(tenantRepoMock.Object);
            repositoryManagerMock.Setup(r => r.Properties).Returns(propertyRepoMock.Object);

            var notificationServiceMock = new Mock<INotificationService>();
            var handler = new CreateBookingCommandHandler(
                repositoryManagerMock.Object, notificationServiceMock.Object);

            var command = new CreateBookingCommand(
                new CreateBookingRequest
                {
                    PropertyId = propertyId,
                    CheckIn = DateTime.UtcNow.AddDays(2),
                    CheckOut = DateTime.UtcNow.AddDays(5),
                    NumberOfGuests = 2
                },
                "1234");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("400", result.Error!.code);
            Assert.Equal("Property not available for booking", result.Error!.message);
        }

        [Fact]
        public async Task Handle_LandlordNotFound_ReturnsFailure()
        {
            var tenant = new TenantProfile
            {
                Id = Guid.NewGuid(),
                AppUser = new AppUser
                {
                    Email = "tenant@example.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    IsActive = true,
                    IsVerified = true
                },
                Bookings = new List<Booking>()
            };

            var propertyId = Guid.NewGuid();
            var landlordProfileId = Guid.NewGuid();
            var property = new Property
            {
                Id = propertyId,
                IsAvailable = true,
                IsApproved = true,
                LandlordProfileId = landlordProfileId,
                PricePerNightKobo = 50000
            };

            var tenantRepoMock = new Mock<ITenantRepository>();
            tenantRepoMock.Setup(r => r.GetTenant(It.IsAny<string>())).ReturnsAsync(tenant);

            var propertyRepoMock = new Mock<IPropertyRepository>();
            propertyRepoMock.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

            var landlordRepoMock = new Mock<ILandlordRepository>();
            landlordRepoMock
                .Setup(r => r.GetByIdAsync(landlordProfileId))
                .ReturnsAsync((LandlordProfile?)null);

            var repositoryManagerMock = new Mock<IRepositoryManager>();
            repositoryManagerMock.Setup(r => r.Tenants).Returns(tenantRepoMock.Object);
            repositoryManagerMock.Setup(r => r.Properties).Returns(propertyRepoMock.Object);
            repositoryManagerMock.Setup(r => r.Landlords).Returns(landlordRepoMock.Object);

            var notificationServiceMock = new Mock<INotificationService>();
            var handler = new CreateBookingCommandHandler(
                repositoryManagerMock.Object, notificationServiceMock.Object);

            var command = new CreateBookingCommand(
                new CreateBookingRequest
                {
                    PropertyId = propertyId,
                    CheckIn = DateTime.UtcNow.AddDays(2),
                    CheckOut = DateTime.UtcNow.AddDays(5),
                    NumberOfGuests = 2
                },
                "1234");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("404", result.Error!.code);
            Assert.Equal("Landlord not found", result.Error!.message);
        }

        [Fact]
        public async Task Handle_DatesAlreadyBooked_ReturnsFailure()
        {
            var tenant = new TenantProfile
            {
                Id = Guid.NewGuid(),
                AppUser = new AppUser
                {
                    Email = "tenant@example.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    IsActive = true,
                    IsVerified = true
                },
                Bookings = new List<Booking>()  
            };

            var propertyId = Guid.NewGuid();
            var landlordProfileId = Guid.NewGuid();
            var property = new Property
            {
                Id = propertyId,
                IsAvailable = true,
                IsApproved = true,
                LandlordProfileId = landlordProfileId,
                PricePerNightKobo = 50000
            };
            var landlord = new LandlordProfile { Id = landlordProfileId };

            var tenantRepoMock = new Mock<ITenantRepository>();
            tenantRepoMock.Setup(r => r.GetTenant(It.IsAny<string>())).ReturnsAsync(tenant);

            var propertyRepoMock = new Mock<IPropertyRepository>();
            propertyRepoMock.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

            var landlordRepoMock = new Mock<ILandlordRepository>();
            landlordRepoMock.Setup(r => r.GetByIdAsync(landlordProfileId)).ReturnsAsync(landlord);

            var bookingRepoMock = new Mock<IBookingRepository>();
            bookingRepoMock
                .Setup(r => r.HasConflictBookingAsync(propertyId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(false);   

            var repositoryManagerMock = new Mock<IRepositoryManager>();
            repositoryManagerMock.Setup(r => r.Tenants).Returns(tenantRepoMock.Object);
            repositoryManagerMock.Setup(r => r.Properties).Returns(propertyRepoMock.Object);
            repositoryManagerMock.Setup(r => r.Landlords).Returns(landlordRepoMock.Object);
            repositoryManagerMock.Setup(r => r.BookingRepository).Returns(bookingRepoMock.Object);

            var notificationServiceMock = new Mock<INotificationService>();
            var handler = new CreateBookingCommandHandler(
                repositoryManagerMock.Object, notificationServiceMock.Object);

            var command = new CreateBookingCommand(
                new CreateBookingRequest
                {
                    PropertyId = propertyId,
                    CheckIn = DateTime.UtcNow.AddDays(2),
                    CheckOut = DateTime.UtcNow.AddDays(5),
                    NumberOfGuests = 2
                },
                "1234");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("400", result.Error!.code);
            Assert.Equal("These dates have been booked", result.Error!.message);
        }
    }
}
