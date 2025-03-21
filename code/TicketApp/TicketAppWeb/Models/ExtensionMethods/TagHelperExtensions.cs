using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TicketAppWeb.Models.ExtensionMethods;

/// <summary>
/// The TagHelperExtensions class provides extension methods for TagHelper classes.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public static class TagHelperExtensions
{

	/// <summary>
	/// Appends the CSS class.
	/// </summary>
	/// <param name="list">The list.</param>
	/// <param name="newCssClasses">The new CSS classes.</param>
	public static void AppendCssClass(this TagHelperAttributeList list,
        string newCssClasses)
    {
        string oldCssClasses = list["class"]?.Value.ToString() ?? "";
        string cssClasses = string.IsNullOrEmpty(oldCssClasses) ?
            newCssClasses : $"{oldCssClasses} {newCssClasses}";
        list.SetAttribute("class", cssClasses);
    }

	/// <summary>
	/// Builds the tag.
	/// </summary>
	/// <param name="output">The output.</param>
	/// <param name="tagName">Name of the tag.</param>
	/// <param name="classNames">The class names.</param>
	public static void BuildTag(this TagHelperOutput output, string tagName,
        string classNames)
    {
        output.TagName = tagName;
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.AppendCssClass(classNames);
    }
}