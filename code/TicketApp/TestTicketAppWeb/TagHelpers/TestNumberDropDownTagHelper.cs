using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TicketAppWeb.TagHelpers;

namespace TestTicketAppWeb.TagHelpers;

/// <summary>
/// Test the tag number drop down tag helper
/// Jabesi Abwe
/// 02/23/2025
/// </summary>
public class TestNumberDropDownTagHelper
{
	[Fact]
	public void Process_GeneratesCorrectOptions()
	{
		// Arrange
		var tagHelper = new NumberDropDownTagHelper
		{
			Min = 1,
			Max = 5
		};

		var tagHelperContext = new TagHelperContext(
			new TagHelperAttributeList
			{
					{ "asp-for", new ModelExpression("Number", CreateModelExplorer(3)) }
			},
			new Dictionary<object, object>(),
			"test");

		var tagHelperOutput = new TagHelperOutput(
			"select",
			new TagHelperAttributeList(),
			(useCachedResult, encoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent("");
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});

		// Act
		tagHelper.Process(tagHelperContext, tagHelperOutput);

		// Assert
		var outputContent = tagHelperOutput.Content.GetContent();
		for (int i = 1; i <= 5; i++)
		{
			Assert.Contains($"<option{(i == 3 ? " selected=\"selected\"" : "")}>{i}</option>", outputContent);
		}
	}

	[Fact]
	public void Process_MinEqualsMax_GeneratesSingleOption()
	{
		// Arrange
		var tagHelper = new NumberDropDownTagHelper
		{
			Min = 5,
			Max = 5
		};

		var tagHelperContext = new TagHelperContext(
			new TagHelperAttributeList
			{
					{ "asp-for", new ModelExpression("Number", CreateModelExplorer(5)) }
			},
			new Dictionary<object, object>(),
			"test");

		var tagHelperOutput = new TagHelperOutput(
			"select",
			new TagHelperAttributeList(),
			(useCachedResult, encoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent("");
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});

		// Act
		tagHelper.Process(tagHelperContext, tagHelperOutput);

		// Assert
		var outputContent = tagHelperOutput.Content.GetContent();
		Assert.Contains("<option selected=\"selected\">5</option>", outputContent);
	}

	private static ModelExplorer CreateModelExplorer(object model)
	{
		var metadataProvider = new EmptyModelMetadataProvider();
		var metadata = metadataProvider.GetMetadataForType(model.GetType());
		var modelExplorer = new ModelExplorer(metadataProvider, metadata, model);
		return modelExplorer;
	}
}