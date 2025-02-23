using TicketAppWeb.Models.ExtensionMethods;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TicketAppWeb.TagHelpers;

/// <summary>
/// The TempMessageTagHelper class.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
[HtmlTargetElement("my-temp-message")]
public class TempMessageTagHelper : TagHelper
{
    // Gets or sets the view CTX.
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewCtx { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var td = ViewCtx.TempData;

        output.TagName = "h4";

        if (td.ContainsKey("message"))
        {
            output.BuildTag("h4", "bg-info text-center text-white p-2");
            output.Content.SetContent(td["message"]?.ToString());
        }
        else if (td.ContainsKey("ErrorMessage"))
        {
            output.BuildTag("h4", "bg-danger text-center text-white p-2");
            output.Content.SetContent(td["ErrorMessage"]?.ToString());
        }
        else
        {
            output.Content.SetContent(string.Empty);
            output.SuppressOutput();
        }
    }
}