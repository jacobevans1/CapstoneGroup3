using TicketAppWeb.Models.Grid;

namespace TestTicketAppWeb;

/// <summary>
/// Test the GridData class, a class that helps with display of the list of x
/// Jabesi Abwe
/// 02/23/2025
/// </summary>
public class TestGridData
{
    [Fact]
    public void GetTotalPages_ReturnsCorrectPages()
    {
        // Arrange
        var gridData = new GridData { PageSize = 5 };

        // Act & Assert
        Assert.Equal(1, gridData.GetTotalPages(3));
        Assert.Equal(2, gridData.GetTotalPages(6));
        Assert.Equal(3, gridData.GetTotalPages(15));
    }

    [Fact]
    public void SetSortAndDirection_SameSortField_AscendingToDescending()
    {
        // Arrange
        var gridData = new GridData();
        var current = new GridData { SortField = "name", SortDirection = "asc" };

        // Act
        gridData.SetSortAndDirection("name", current);

        // Assert
        Assert.Equal("desc", gridData.SortDirection);
    }

    [Fact]
    public void SetSortAndDirection_DifferentSortField_AlwaysAscending()
    {
        // Arrange
        var gridData = new GridData();
        var current = new GridData { SortField = "name", SortDirection = "desc" };

        // Act
        gridData.SetSortAndDirection("date", current);

        // Assert
        Assert.Equal("asc", gridData.SortDirection);
    }

    [Fact]
    public void Clone_ReturnsDeepCopy()
    {
        // Arrange
        var gridData = new GridData { PageNumber = 2, PageSize = 10, SortField = "name", SortDirection = "desc" };

        // Act
        var clone = gridData.Clone();

        // Assert
        Assert.Equal(gridData.PageNumber, clone.PageNumber);
        Assert.Equal(gridData.PageSize, clone.PageSize);
        Assert.Equal(gridData.SortField, clone.SortField);
        Assert.Equal(gridData.SortDirection, clone.SortDirection);
    }

    [Fact]
    public void ToDictionary_ReturnsCorrectDictionary()
    {
        // Arrange
        var gridData = new GridData { PageNumber = 2, PageSize = 10, SortField = "name", SortDirection = "desc" };

        // Act
        var dictionary = gridData.ToDictionary();

        // Assert
        Assert.Equal("2", dictionary["PageNumber"]);
        Assert.Equal("10", dictionary["PageSize"]);
        Assert.Equal("name", dictionary["SortField"]);

    }
}
