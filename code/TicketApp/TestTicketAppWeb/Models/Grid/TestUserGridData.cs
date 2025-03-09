using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;

namespace TestTicketAppWeb.Models.Grid
{
	public class TestUserGridData
	{
		[Fact]
		public void Constructor_SetsDefaultSortFieldToLastName()
		{
			// Act
			var gridData = new UserGridData();

			// Assert
			Assert.Equal(nameof(TicketAppUser.LastName), gridData.SortField);
		}

		[Fact]
		public void IsSortByFirstName_ReturnsTrue_WhenSortFieldIsFirstName()
		{
			// Arrange
			var gridData = new UserGridData { SortField = nameof(TicketAppUser.FirstName) };

			// Act
			var result = gridData.IsSortByFirstName;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsSortByFirstName_ReturnsFalse_WhenSortFieldIsNotFirstName()
		{
			// Arrange
			var gridData = new UserGridData { SortField = nameof(TicketAppUser.LastName) };

			// Act
			var result = gridData.IsSortByFirstName;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void IsSortByFirstName_IgnoresCaseWhenComparingSortField()
		{
			// Arrange
			var gridData = new UserGridData { SortField = "firstname" };

			// Act
			var result = gridData.IsSortByFirstName;

			// Assert
			Assert.True(result);
		}
	}
}
