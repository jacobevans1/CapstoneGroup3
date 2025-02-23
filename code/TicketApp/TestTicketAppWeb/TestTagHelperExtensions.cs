using Microsoft.AspNetCore.Razor.TagHelpers;
using TicketAppWeb.Models.ExtensionMethods;
namespace TestTicketAppWeb;

/// <summary>
/// Test the Tag Helper Extensions Methods
/// Jabesi Abwe
/// 02/23/2025
/// </summary>
public class TestTagHelperExtensions
{
    [Fact]
    public void AppendCssClass_AppendsNewClass()
    {
        // Arrange
        var attributes = new TagHelperAttributeList();

        attributes.SetAttribute("class", "existing-class");

        // Act
        attributes.AppendCssClass("new-class");

        // Assert
        Assert.Equal("existing-class new-class", attributes["class"].Value.ToString());
    }

    [Fact]
    public void AppendCssClass_AddsClassWhenNoneExist()
    {
        // Arrange
        var attributes = new TagHelperAttributeList();

        // Act
        attributes.AppendCssClass("new-class");

        // Assert
        Assert.Equal("new-class", attributes["class"].Value.ToString());
    }



    [Fact]
    public void BuildTag_SetsTagNameAndClass()
    {
        // Arrange
        var output = new TagHelperOutput("div", new TagHelperAttributeList(), (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        output.BuildTag("span", "test-class");

        // Assert
        Assert.Equal("span", output.TagName);

        Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);

        Assert.Equal("test-class", output.Attributes["class"].Value.ToString());
    }



    [Fact]
    public void BuildLink_SetsTagNameHrefAndClass()
    {
        // Arrange
        var output = new TagHelperOutput("div", new TagHelperAttributeList(), (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        output.BuildLink("http://example.com", "link-class");

        // Assert
        Assert.Equal("a", output.TagName);

        Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);

        Assert.Equal("link-class", output.Attributes["class"].Value.ToString());

        Assert.Equal("http://example.com", output.Attributes["href"].Value.ToString());
    }
}
