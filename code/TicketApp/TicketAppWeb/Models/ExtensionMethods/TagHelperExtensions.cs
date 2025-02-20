using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TicketAppWeb.Models.ExtensionMethods;

/// <summary>
/// The TagHelperExtensions class provides extension methods for TagHelper classes.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public static class TagHelperExtensions
{

    // Appends the CSS class.
    public static void AppendCssClass(this TagHelperAttributeList list,
        string newCssClasses)
    {
        string oldCssClasses = list["class"]?.Value.ToString() ?? "";
        string cssClasses = string.IsNullOrEmpty(oldCssClasses) ?
            newCssClasses : $"{oldCssClasses} {newCssClasses}";
        list.SetAttribute("class", cssClasses);
    }

    // Builds the tag.
    public static void BuildTag(this TagHelperOutput output, string tagName,
        string classNames)
    {
        output.TagName = tagName;
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.AppendCssClass(classNames);
    }

    // Builds the link.
    public static void BuildLink(this TagHelperOutput output, string url,
        string classNames)
    {
        output.BuildTag("a", classNames);
        output.Attributes.SetAttribute("href", url);
    }
}