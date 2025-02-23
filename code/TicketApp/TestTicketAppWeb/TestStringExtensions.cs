using TicketAppWeb.Models.ExtensionMethods;

namespace TestTicketAppWeb;

/// <summary>
/// Test the string extension methods
/// Jabesi Abwe
/// 02/23/2025
/// </summary>
public class TestStringExtensions
{
    [Fact]
    public void EqualsNoCase_StringsAreEqualIgnoringCase_ReturnsTrue()
    {
        // Arrange
        string str1 = "hello";
        string str2 = "HELLO";

        // Act
        bool result = str1.EqualsNoCase(str2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EqualsNoCase_StringsAreNotEqual_ReturnsFalse()
    {
        // Arrange
        string str1 = "hello";
        string str2 = "world";

        // Act
        bool result = str1.EqualsNoCase(str2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EqualsNoCase_OneStringIsNull_ReturnsFalse()
    {
        // Arrange
        string str1 = null!;
        string str2 = "world";

        // Act
        bool result = str1.EqualsNoCase(str2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ToInt_ValidIntegerString_ReturnsInteger()
    {
        // Arrange
        string str = "123";

        // Act
        int result = str.ToInt();

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void ToInt_InvalidIntegerString_ReturnsZero()
    {
        // Arrange
        string str = "abc";

        // Act
        int result = str.ToInt();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Capitalize_NonEmptyString_ReturnsCapitalizedString()
    {
        // Arrange
        string str = "hello";

        // Act
        string result = str.Capitalize();

        // Assert
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Capitalize_EmptyString_ReturnsEmptyString()
    {
        // Arrange
        string str = "";

        // Act
        string result = str.Capitalize();

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void Capitalize_NullString_ReturnsNull()
    {
        // Arrange
        string str = null!;

        // Act
        string result = str.Capitalize();

        // Assert
        Assert.Null(result);
    }
}
