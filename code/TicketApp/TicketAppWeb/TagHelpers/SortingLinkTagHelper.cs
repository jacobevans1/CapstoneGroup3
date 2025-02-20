using TicketAppWeb.Models.ExtensionMethods;
using TicketAppWeb.Models.Grid;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TicketAppWeb.TagHelpers;

/// <summary>
/// The SortingLinkTagHelper class creates a link for sorting the grid.
/// Jabesi Abwe
/// 07/04/2024
/// </summary>
[HtmlTargetElement("my-sorting-link")]
public class SortingLinkTagHelper : TagHelper
{
    private LinkGenerator linkBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortingLinkTagHelper"/> class.
    /// </summary>
    /// <param name="lg">The lg.</param>
    public SortingLinkTagHelper(LinkGenerator lg) => linkBuilder = lg;

    /// <summary>
    /// Gets or sets the view CTX.
    /// </summary>
    /// <value>
    /// The view CTX.
    /// </value>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewCtx { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current.
    /// </summary>
    /// <value>
    /// The current.
    /// </value>
    public GridData Current { get; set; } = null!;

    /// <summary>
    /// Gets or sets the sort field.
    /// </summary>
    /// <value>
    /// The sort field.
    /// </value>
    public string SortField { get; set; } = string.Empty;

    // Synchronously executes the Microsoft AspNetCore Razor TagHelpers with the given param
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