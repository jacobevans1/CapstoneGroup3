using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestGroup
	{
		[Fact]
		public void GroupConstructor()
		{
			//Arrange
			Group group = new Group();

			//Assert
			Assert.NotNull(group.Id);
			Assert.NotNull(group.Members);
			Assert.NotNull(group.Projects);
			Assert.NotEqual(DateTime.MinValue, group.CreatedAt);
		}

		[Fact]
		public void GetGroupManager()
		{
			//Arrange
			Group group = new Group();
			TicketAppUser manager = new TicketAppUser();
			group.Manager = manager;
			
			//Act
			TicketAppUser result = group.Manager;
			
			//Assert
			Assert.Equal(manager, result);
		}

		[Fact]
		public void SetGroupManager()
		{
			//Arrange
			Group group = new Group();
			TicketAppUser manager = new TicketAppUser();

			//Act
			group.Manager = manager;

			//Assert
			Assert.Equal(manager, group.Manager);
		}
	}
}
