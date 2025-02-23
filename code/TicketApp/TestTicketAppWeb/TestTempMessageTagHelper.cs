using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using TicketAppWeb.TagHelpers;

namespace TestTicketAppWeb;

/// <summary>
/// Tests the TempMessageTagHelper
/// Jabesi Abwe 
/// 02/23/2024
/// </summary>
public class TestTempMessageTagHelper
{
    private static TempDataDictionary CreateTempData(Dictionary<string, object?> values)
    {
        var httpContext = new DefaultHttpContext();
        var tempDataProviderMock = new Mock<ITempDataProvider>();
        var tempData = new TempDataDictionary(httpContext, tempDataProviderMock.Object);

        foreach (var kvp in values)
        {
            tempData[kvp.Key] = kvp.Value;
        }

        return tempData;
    }

    [Theory]
    [InlineData("message", "Success!", "bg-info text-center text-white p-2")]
    [InlineData("message", "", "bg-info text-center text-white p-2")] 
    [InlineData("message", null, "bg-info text-center text-white p-2")]
    [InlineData("ErrorMessage", "Error occurred", "bg-danger text-center text-white p-2")]
    [InlineData("ErrorMessage", "", "bg-danger text-center text-white p-2")]
    [InlineData("ErrorMessage", null, "bg-danger text-center text-white p-2")]
    [InlineData("", null, "")]
    public void Process_ShouldSetCorrectMessageAndClasses(string key, string? message, string expectedClass)
    {
        // Arrange
        var tagHelper = new TempMessageTagHelper
        {
            ViewCtx = new ViewContext { TempData = CreateTempData(new Dictionary<string, object?> { { key, message } }) }
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput(
            "my-temp-message",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
        );

        // Act
        tagHelper.Process(context, output);

        // Assert
        if (key == "message" && (message == null || message == ""))
        {
            Assert.Equal("h4", output.TagName);
            Assert.Contains(expectedClass, output.Attributes["class"]?.Value.ToString());
            Assert.Equal(string.Empty, output.Content.GetContent());
        }
        else if (key == "ErrorMessage" && (message == null || message == ""))
        {
            Assert.Equal("h4", output.TagName);
            Assert.Contains(expectedClass, output.Attributes["class"]?.Value.ToString());
            Assert.Equal(string.Empty, output.Content.GetContent());
        }
        else if (string.IsNullOrEmpty(key) && message == null)
        {
            Assert.Null(output.TagName);
            Assert.Empty(output.Content.GetContent());
        }
        else
        {
            Assert.Equal("h4", output.TagName);
            Assert.Contains(expectedClass, output.Attributes["class"]?.Value.ToString());
            Assert.Equal(message, output.Content.GetContent());
        }
    }

    [Fact]
    public void Process_ShouldSuppressOutput_WhenNoMessage()
    {
        // Arrange
        var tagHelper = new TempMessageTagHelper
        {
            ViewCtx = new ViewContext { TempData = CreateTempData(new Dictionary<string, object?>()) }
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput(
            "my-temp-message",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
        );

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Null(output.TagName);
        Assert.Empty(output.Content.GetContent());
    }
}

