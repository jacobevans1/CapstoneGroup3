using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TicketAppWeb.TagHelpers;

/// <summary>
/// The NumberDropDownTagHelper class is a tag helper class that creates a dropdown list of numbers.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
[HtmlTargetElement("select", Attributes = "my-min-number, my-max-number")]
public class NumberDropDownTagHelper : TagHelper
{
	[HtmlAttributeName("my-min-number")]
	public int Min { get; set; }
	[HtmlAttributeName("my-max-number")]
	public int Max { get; set; }

	public override void Process(TagHelperContext context,
		TagHelperOutput output)
	{
		// get selected value from view's model
		ModelExpression aspfor =
			(ModelExpression)context.AllAttributes["asp-for"].Value;
		int modelValue = (int)aspfor.Model;

		for (int i = Min; i <= Max; i++)
		{
			TagBuilder option = new TagBuilder("option");
			option.InnerHtml.Append(i.ToString());

			// mark option as selected if matches model’s value
			if (modelValue == i)
				option.Attributes["selected"] = "selected";

			output.Content.AppendHtml(option);
		}
	}
}