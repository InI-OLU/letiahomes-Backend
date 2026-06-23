using letiahomes.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace letiahomes.Application.Tests.TestHelpers
{
    public static class TestUserManagerFactory
    {
        public static Mock<UserManager<AppUser>> Create()
        {
            var userStoreMock = new Mock<IUserStore<AppUser>>();

            var userManagerMock = new Mock<UserManager<AppUser>>(
                userStoreMock.Object,
                null!, null!, null!, null!, null!, null!, null!, null!);

            return userManagerMock;
        }
    }
}