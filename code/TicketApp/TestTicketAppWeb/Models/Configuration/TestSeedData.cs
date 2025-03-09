using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using TicketAppWeb.Models.Configuration;
using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Models.Configuration
{
	public class TestSeedData
	{
		[Fact]
		public async Task Initialize_CreatesAdminUserIfNotExists()
		{
			// Arrange
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(
				Mock.Of<IUserStore<TicketAppUser>>(), null, null, null, null, null, null, null, null);

			userManagerMock.Setup(u => u.FindByEmailAsync("admin@domain.com")).ReturnsAsync((TicketAppUser)null);
			userManagerMock.Setup(u => u.CreateAsync(It.IsAny<TicketAppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<TicketAppUser>(), "Admin")).ReturnsAsync(IdentityResult.Success);

			var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
			var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
				roleStoreMock.Object,
				new List<IRoleValidator<IdentityRole>>(),
				new Mock<ILookupNormalizer>().Object,
				new IdentityErrorDescriber(),
				new Mock<ILogger<RoleManager<IdentityRole>>>().Object
			);

			var serviceProvider = Mock.Of<IServiceProvider>();

			// Act
			await SeedData.Initialize(serviceProvider, userManagerMock.Object, roleManagerMock.Object);

			// Assert
			userManagerMock.Verify(u => u.CreateAsync(It.Is<TicketAppUser>(user => user.Email == "admin@domain.com"), "Admin123!"), Times.Once);
			userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<TicketAppUser>(), "Admin"), Times.Once);
		}

		[Fact]
		public async Task CreateUserIfNotExists_CreatesUser_WhenUserDoesNotExist()
		{
			// Arrange
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(
				Mock.Of<IUserStore<TicketAppUser>>(), null, null, null, null, null, null, null, null);

			userManagerMock.Setup(u => u.FindByEmailAsync("test@domain.com")).ReturnsAsync((TicketAppUser)null);
			userManagerMock.Setup(u => u.CreateAsync(It.IsAny<TicketAppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<TicketAppUser>(), "User")).ReturnsAsync(IdentityResult.Success);

			var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
			var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
				roleStoreMock.Object,
				new List<IRoleValidator<IdentityRole>>(),
				new Mock<ILookupNormalizer>().Object,
				new IdentityErrorDescriber(),
				new Mock<ILogger<RoleManager<IdentityRole>>>().Object
			);

			// Act
			await SeedData.Initialize(Mock.Of<IServiceProvider>(), userManagerMock.Object, roleManagerMock.Object);

			// Assert
			userManagerMock.Verify(u => u.CreateAsync(It.Is<TicketAppUser>(user => user.Email == "jabesi@domain.com"), "JabesiAbwe123!"), Times.Once);
			userManagerMock.Verify(u => u.CreateAsync(It.Is<TicketAppUser>(user => user.Email == "jacob@domain.com"), "JacobEvans123!"), Times.Once);
		}
	}
}
