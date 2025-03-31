using Microsoft.AspNetCore.Razor.TagHelpers;
using TicketAppWeb.Models.ExtensionMethods;

namespace TestTicketAppWeb.ExtensionMethods;

public class TestTagHelperExtensions
{
	[Fact]
	public void AppendCssClass_AppendsNewClass_WhenNoExistingClass()
	{
		// Arrange
		var attributes = new TagHelperAttributeList();

		// Act
		attributes.AppendCssClass("new-class");

		// Assert
		Assert.Equal("new-class", attributes["class"]?.Value);
	}

	[Fact]
	public void AppendCssClass_AppendsNewClass_WhenExistingClass()
	{
		// Arrange
		var attributes = new TagHelperAttributeList { { "class", "existing-class" } };

		// Act
		attributes.AppendCssClass("new-class");

		// Assert
		Assert.Equal("existing-class new-class", attributes["class"]?.Value);
	}

	[Fact]
	public void BuildTag_SetsTagNameAndClass()
	{
		// Arrange
		var output = new TagHelperOutput(
			"test",
			new TagHelperAttributeList(),
			(useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
		);

		// Act
		output.BuildTag("div", "test-class");

		// Assert
		Assert.Equal("div", output.TagName);
		Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
		Assert.Equal("test-class", output.Attributes["class"]?.Value);
	}
}