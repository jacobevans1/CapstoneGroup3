using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestSingletonService
	{
		[Fact]
		public void SingletonService_InitializesWithUniqueId()
		{
			// Act
			var service1 = new SingletonService();
			var service2 = new SingletonService();

			// Assert
			Assert.NotEqual(Guid.Empty, service1.Id);
			Assert.NotEqual(Guid.Empty, service2.Id);
			Assert.NotEqual(service1.Id, service2.Id);
		}

		[Fact]
		public void GetMessage_ReturnsExpectedMessage()
		{
			// Arrange
			var service = new SingletonService();
			var expectedMessage = $"SingletonService with Id: {service.Id}";

			// Act
			var message = service.GetMessage();

			// Assert
			Assert.Equal(expectedMessage, message);
		}

		[Fact]
		public void CanSetAndGetCurrentUser()
		{
			// Arrange
			var service = new SingletonService();
			var user = new TicketAppUser { UserName = "testuser" };

			// Act
			service.CurrentUser = user;

			// Assert
			Assert.Equal(user, service.CurrentUser);
		}

		[Fact]
		public void CanSetAndGetCurrentUserRole()
		{
			// Arrange
			var service = new SingletonService();

			// Act
			service.CurrentUserRole = "Admin";

			// Assert
			Assert.Equal("Admin", service.CurrentUserRole);
		}
	}
}
