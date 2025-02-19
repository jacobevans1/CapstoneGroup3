using System.Text.Json.Serialization;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ExtensionMethods;

namespace TicketAppWeb.Models.Grid;

/// <summary>
/// The VacationGridData class represents the data for the vacation grid.
/// Jabesi Abwe
/// 02/19/2025
/// </summary>
public class ProjectGridData : GridData
{
    // set initial sort field in constructor
    public ProjectGridData() => SortField = nameof(DomainModels.Project.ProjectName);

    // sort flags
    [JsonIgnore]
    public bool IsSortByProjectLead =>
        SortField.EqualsNoCase(nameof(TicketAppUser));
    [JsonIgnore]
    public bool IsSortByCreatedBy =>
        SortField.EqualsNoCase(nameof(TicketAppUser));

    [JsonIgnore]
    public bool IsSortByDateCreated =>
        SortField.EqualsNoCase(nameof(DomainModels.Project.CreatedOnDate));

}