using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Models.ViewModels;

/// <summary>
/// The the helper class that highlights the active nav
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

	//[Fact]
	//public void Active_StringValuesEqual_ReturnsNavActive()
	//{
	//	// Arrange
	//	string value = "Home";

	//	string current = "home";

	//	// Act
	//	string result = Nav.Active(value, current);

	//	// Assert
	//	Assert.Equal("nav-active", result);
	//}

	//[Fact]

	//public void Active_StringValuesNotEqual_ReturnsEmptyString()

	//{
	//	// Arrange

	//	string value = "Home";

	//	string current = "About";

	//	// Act
	//	string result = Nav.Active(value, current);

	//	// Assert
	//	Assert.Equal("", result);

	//}
}

