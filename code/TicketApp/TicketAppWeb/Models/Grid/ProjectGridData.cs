using System.Text.Json.Serialization;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ExtensionMethods;

namespace TicketAppWeb.Models.Grid;

/// <summary>
/// The ProjectGridData class represents the data for the project grid.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public class ProjectGridData : GridData
{
    // set initial sort field in constructor
    public ProjectGridData() => SortField = nameof(Project.ProjectName);

    // sort flags
    [JsonIgnore]
    public bool IsSortByProjectLead =>
        SortField.EqualsNoCase(nameof(TicketAppUser));
}