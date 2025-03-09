using TicketAppWeb.Models.ViewModels;
namespace TicketAppWeb.Tests.Models.ViewModels;

/// <summary>
/// The helper class that highlights the active nav
/// Jabesi Abwe
/// 02/23/2025
/// </summary>
public class NavTests
{
    [Fact]
    public void Active_IntValuesEqual_ReturnsActive()
    {
        // Arrange
        int value = 5;
        int current = 5;

        // Act
        string result = Nav.Active(value, current);

        // Assert
        Assert.Equal("active", result);
    }

    [Fact]
    public void Active_IntValuesNotEqual_ReturnsEmptyString()
    {
        // Arrange
        int value = 5;
        int current = 10;

        // Act
        string result = Nav.Active(value, current);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void Active_StringValuesEqual_ReturnsNavActive()
    {
        // Arrange
        string expectedController = "Home";
        string currentController = "home";
        string expectedAction = "Index";
        string currentAction = "index";

        // Act
        string result = Nav.Active(expectedController, currentController, expectedAction, currentAction);

        // Assert
        Assert.Equal("nav-active", result);
    }

    [Fact]
    public void Active_StringValuesNotEqual_ReturnsEmptyString()
    {
        // Arrange
        string expectedController = "Home";
        string currentController = "About";
        string expectedAction = "Index";
        string currentAction = "Contact";

        // Act
        string result = Nav.Active(expectedController, currentController, expectedAction, currentAction);

        // Assert
        Assert.Equal("", result);
    }
}