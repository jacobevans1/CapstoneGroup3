using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Diagnostics.CodeAnalysis;
using TicketAppWeb.Models.ExtensionMethods;
using TicketAppWeb.Models.Grid;

namespace TicketAppWeb.TagHelpers;

/// <summary>
/// The SortingLinkTagHelper class creates a link for sorting the grid.
/// Jabesi Abwe
/// 02/20/2025
/// </summary>
[ExcludeFromCodeCoverage]
[HtmlTargetElement("my-sorting-link")]
public class SortingLinkTagHelper : TagHelper
{
    private LinkGenerator linkBuilder;

    // Initializes a new instance of the SortingLinkTagHelper class.
    public SortingLinkTagHelper(LinkGenerator lg) => linkBuilder = lg;

    // Gets or sets the view CTX.
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewCtx { get; set; } = null!;

    // Gets or sets the current.
    public GridData Current { get; set; } = null!;

    // Gets or sets the sort field.
     public string SortField { get; set; } = string.Empty;

    // Synchronously executes the Microsoft AspNetCore Razor TagHelpers TagHelper with the given param
    public override void Process(TagHelperContext context,
        TagHelperOutput output)
    {
        var routes = Current.Clone();
        routes.SetSortAndDirection(SortField, Current);

        string ctlr = ViewCtx.RouteData.Values["controller"]?.ToString() ?? "";
        string action = ViewCtx.RouteData.Values["action"]?.ToString() ?? "";
        string url = linkBuilder.GetPathByAction(
            action, ctlr, routes.ToDictionary()) ?? "";

        output.BuildLink(url, "text-black text-decoration-none");
    }
}