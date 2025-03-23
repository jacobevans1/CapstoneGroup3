using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using TicketAppWeb.TagHelpers;
using Xunit;

namespace TestTicketAppWeb.TagHelpers
{
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
        [InlineData("SuccessMessage", "Success!", "bg-success bg-opacity-25 text-center text-success p-2")]
        [InlineData("SuccessMessage", "", "bg-success bg-opacity-25 text-center text-success p-2")]
        [InlineData("ErrorMessage", "Error occurred", "bg-danger bg-opacity-25 text-center text-danger p-2")]
        [InlineData("ErrorMessage", "", "bg-danger bg-opacity-25 text-center text-danger p-2")]
        [InlineData("ErrorMessage", null, "bg-danger bg-opacity-25 text-center text-danger p-2")]
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
            if (key == "SuccessMessage" && (message == null || message == ""))
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

        [Fact]
        public void Process_ShouldHandleNullSuccessMessage()
        {
            // Arrange
            var tagHelper = new TempMessageTagHelper
            {
                ViewCtx = new ViewContext { TempData = CreateTempData(new Dictionary<string, object?> { { "SuccessMessage", null } }) }
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
            Assert.Equal("h4", output.TagName);
            Assert.Contains("bg-success bg-opacity-25 text-center text-success p-2", output.Attributes["class"]?.Value.ToString());
            Assert.Equal(string.Empty, output.Content.GetContent());
        }
    }
}