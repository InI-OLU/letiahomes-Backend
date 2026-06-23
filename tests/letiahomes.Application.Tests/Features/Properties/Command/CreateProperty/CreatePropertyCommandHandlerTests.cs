using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.Features.Properties.Command.CreateProperty;
using letiahomes.Application.Tests.TestHelpers;
using letiahomes.Domain.Entities;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace letiahomes.Application.Tests.Features.Properties.Command.CreateProperty
{
    public class CreatePropertyCommandHandlerTests
    {
        [Fact]
        public async Task Handle_LandlordNotFoundOrUnverified_ReturnsFailure()
        {
            // Arrange
            var emptyLandlords = new List<LandlordProfile>().AsQueryable().BuildMock();

            var landlordRepoMock = new Mock<ILandlordRepository>();
            landlordRepoMock
                .Setup(r => r.Get(It.IsAny<Expression<Func<LandlordProfile, bool>>>(), It.IsAny<bool>()))
                .Returns(emptyLandlords);

            var repositoryManagerMock = new Mock<IRepositoryManager>();
            repositoryManagerMock
                .Setup(r => r.Landlords)
                .Returns(landlordRepoMock.Object);

            var userManagerMock = TestUserManagerFactory.Create();

            var handler = new CreatePropertyCommandHandler(
                repositoryManagerMock.Object,
                userManagerMock.Object);

            var command = new CreatePropertyCommand(
       new CreatePropertyRequest
       {
           Title = "Test Apartment",
           City = "Lagos",
           State = "Lagos",
           Description = "A nice place",
           Address = "123 Test St",
           MaxGuests = 4,
           Bathrooms = 2,
           Bedrooms = 2,
           PricePerNightKobo = 50000,
           PropertyType = default,
           ListingType = default
       },
       "some-user-id");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("404", result.Value);
        }
    }
}