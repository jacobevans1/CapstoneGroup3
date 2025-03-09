using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestTicketAppUser
	{
		[Fact]
		public void GetUserFullName()
		{
			//Arrange
			string firstName = "John";
			string lastName = "Doe";

			//Act
			TicketAppUser user = new TicketAppUser
			{
				FirstName = firstName,
				LastName = lastName
			};

			//Assert
			Assert.Equal($"{firstName} {lastName}", user.FullName);
		}

		[Fact]
		public void GetUserFullName_Empty()
		{
			//Arrange
			string firstName = string.Empty;
			string lastName = string.Empty;
			//Act
			TicketAppUser user = new TicketAppUser
			{
				FirstName = firstName,
				LastName = lastName
			};
			//Assert
			Assert.Equal($"{firstName} {lastName}", user.FullName);
		}

		[Fact]
		public void GetUserFullName_Null()
		{
			//Arrange
			string firstName = null;
			string lastName = null;
			//Act
			TicketAppUser user = new TicketAppUser
			{
				FirstName = firstName,
				LastName = lastName
			};
			//Assert
			Assert.Equal($"{firstName} {lastName}", user.FullName);
		}
	}
}
