using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories;
using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Models.DataLayer
{
	public class UserRepositoryTests
	{
		private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> elements) where T : class
		{
			var queryable = elements.AsQueryable();
			var mockSet = new Mock<DbSet<T>>();
			mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
			return mockSet;
		}

		private TicketAppContext CreateMockContext<T>(List<T> data, string dbName) where T : class
		{
			var options = new DbContextOptionsBuilder<TicketAppContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;
			var context = new TicketAppContext(options);
			var mockSet = CreateMockDbSet(data);
			context.Set<T>().AddRange(data);
			context.SaveChanges();
			return context;
		}

		[Fact]
		public async Task CreateUser_UserAlreadyExists_ThrowsException()
		{
			// Arrange
			var user = new TicketAppUser { UserName = "existingUser", FirstName = "John", LastName = "Doe" };
			var users = new List<TicketAppUser> { user };
			var context = CreateMockContext(users, "TestDatabase_UserAlreadyExists");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(() => repository.CreateUser(user, "Role"));
		}

		[Fact]
		public async Task CreateUser_UserCreationFails_ThrowsException()
		{
			// Arrange
			var user = new TicketAppUser { UserName = "newUser", FirstName = "John", LastName = "Doe" };
			var users = new List<TicketAppUser>();
			var context = CreateMockContext(users, "TestDatabase_UserCreationFails");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			userManagerMock.Setup(um => um.CreateAsync(It.IsAny<TicketAppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Creation failed" }));
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act & Assert
			var exception = await Assert.ThrowsAsync<Exception>(() => repository.CreateUser(user, "Role"));
			Assert.Contains("Creation failed", exception.Message);
		}

		[Fact]
		public async Task CreateUser_UserCreationSucceeds_AddsRole()
		{
			// Arrange
			var user = new TicketAppUser { UserName = "newUser", FirstName = "John", LastName = "Doe" };
			var users = new List<TicketAppUser>();
			var context = CreateMockContext(users, "TestDatabase_UserCreationSucceeds");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			userManagerMock.Setup(um => um.CreateAsync(It.IsAny<TicketAppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<TicketAppUser>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act
			await repository.CreateUser(user, "Role");

			// Assert
			userManagerMock.Verify(um => um.AddToRoleAsync(user, "Role"), Times.Once);
		}

		[Fact]
		public async Task UpdateUser_UserDoesNotExist_DoesNothing()
		{
			// Arrange
			var user = new TicketAppUser { Id = "nonexistentUser", FirstName = "John", LastName = "Doe" };
			var users = new List<TicketAppUser>();
			var context = CreateMockContext(users, "TestDatabase_UserDoesNotExist");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act
			await repository.UpdateUser(user, "Role");

			// Assert
			userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<TicketAppUser>()), Times.Never);
		}

		[Fact]
		public async Task UpdateUser_UserExists_UpdatesUserAndRole()
		{
			// Arrange
			var user = new TicketAppUser { Id = "existingUser", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
			var users = new List<TicketAppUser> { user };
			var context = CreateMockContext(users, "TestDatabase_UserExists");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			userManagerMock.Setup(um => um.FindByIdAsync("existingUser")).ReturnsAsync(user);
			userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<TicketAppUser>())).ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<TicketAppUser>())).ReturnsAsync(new List<string> { "OldRole" });
			userManagerMock.Setup(um => um.RemoveFromRolesAsync(It.IsAny<TicketAppUser>(), It.IsAny<IEnumerable<string>>())).Returns(Task.FromResult(IdentityResult.Success));
			userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<TicketAppUser>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act
			await repository.UpdateUser(user, "NewRole");

			// Assert
			userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
			userManagerMock.Verify(um => um.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Once);
			userManagerMock.Verify(um => um.AddToRoleAsync(user, "NewRole"), Times.Once);
		}

		[Fact]
		public async Task GetRolesAsync_ReturnsAllRoles()
		{
			// Arrange
			var roles = new List<IdentityRole>
		{
			new IdentityRole { Name = "Role1" },
			new IdentityRole { Name = "Role2" }
		};
			var context = CreateMockContext(roles, "TestDatabase_GetRolesAsync");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act
			var result = await repository.GetRolesAsync();

			// Assert
			Assert.Equal(2, result.Count());
			Assert.Contains(result, r => r.Name == "Role1");
			Assert.Contains(result, r => r.Name == "Role2");
		}

		[Fact]
		public async Task GetUserRolesAsync_ReturnsUserRoles()
		{
			// Arrange
			var users = new List<TicketAppUser>
		{
			new TicketAppUser { UserName = "User1" },
			new TicketAppUser { UserName = "User2" }
		};
			var context = CreateMockContext(users, "TestDatabase_GetUserRolesAsync");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<TicketAppUser>())).ReturnsAsync(new List<string> { "Role1" });
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act
			var result = await repository.GetUserRolesAsync();

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Equal("Role1", result[users[0]]);
			Assert.Equal("Role1", result[users[1]]);
		}

		[Fact]
		public async Task GetUserRolesAsync_UserHasNoRole_ReturnsNoRole()
		{
			// Arrange
			var users = new List<TicketAppUser>
	{
		new TicketAppUser { UserName = "User1" },
		new TicketAppUser { UserName = "User2" }
	};
			var context = CreateMockContext(users, "TestDatabase_GetUserRolesAsync_NoRole");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<TicketAppUser>())).ReturnsAsync(new List<string>());
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act
			var result = await repository.GetUserRolesAsync();

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Equal("No Role", result[users[0]]);
			Assert.Equal("No Role", result[users[1]]);
		}

		[Fact]
		public async Task GetAllUsersAsync_ReturnsAllUsers()
		{
			// Arrange
			var users = new List<TicketAppUser>
		{
			new TicketAppUser { UserName = "User1" },
			new TicketAppUser { UserName = "User2" }
		};
			var context = CreateMockContext(users, "TestDatabase_GetAllUsersAsync");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act
			var result = await repository.GetAllUsersAsync();

			// Assert
			Assert.Equal(2, result.Count());
			Assert.Contains(result, u => u.UserName == "User1");
			Assert.Contains(result, u => u.UserName == "User2");
		}

		[Fact]
		public async Task GetAsync_ReturnsUserById()
		{
			// Arrange
			var user = new TicketAppUser { Id = "userId", UserName = "User1" };
			var users = new List<TicketAppUser> { user };
			var context = CreateMockContext(users, "TestDatabase_GetAsync");
			var userManagerMock = new Mock<UserManager<TicketAppUser>>(Mock.Of<IUserStore<TicketAppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
			var repository = new UserRepository(context, userManagerMock.Object);

			// Act
			var result = await repository.GetAsync("userId");

			// Assert
			Assert.NotNull(result);
			Assert.Equal("User1", result.UserName);
		}
	}
}