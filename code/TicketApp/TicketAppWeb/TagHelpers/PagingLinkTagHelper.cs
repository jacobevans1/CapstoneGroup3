using TicketAppWeb.Models.ExtensionMethods;
using TicketAppWeb.Models.Grid;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TicketAppWeb.TagHelpers;

/// <summary>
/// The PagingLinkTagHelper class creates a paging link for the grid.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
[HtmlTargetElement("my-paging-link")]
public class PagingLinkTagHelper : TagHelper
{
    private LinkGenerator linkBuilder;
    public PagingLinkTagHelper(LinkGenerator lg) => linkBuilder = lg;

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewCtx { get; set; } = null!;

    public int Number { get; set; }
    public GridData Current { get; set; } = null!;

    public override void Process(TagHelperContext context,
        TagHelperOutput output)
    {
        // update routes for this paging link
        var routes = Current.Clone();
        routes.PageNumber = Number;

        // get controller and action method, create paging link URL
        string ctlr = ViewCtx.RouteData.Values["controller"]?.ToString() ?? "";
        string action = ViewCtx.RouteData.Values["action"]?.ToString() ?? "";
        string url = linkBuilder.GetPathByAction(
            action, ctlr, routes.ToDictionary()) ?? "";

        // build up CSS string
        string linkClasses = "btn btn-outline-primary";
        if (Number == Current.PageNumber)
            linkClasses += " active";

        // create link
        output.BuildLink(url, linkClasses);
        output.Content.SetContent(Number.ToString());
    }
}