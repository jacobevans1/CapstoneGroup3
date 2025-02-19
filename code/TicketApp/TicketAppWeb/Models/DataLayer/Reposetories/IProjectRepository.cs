using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Reposetories
{
    /// <summary>
    /// The IProjectRepository interface defines the methods that must be implemented by all Vacation repository classes.
    /// Jabesi Abwe
    /// 02/19/2025
    /// </summary>
    /// <seealso cref="AbweVacationPlanner.Models.DataLayer.Repositories.IRepository&lt;AbweVacationPlanner.Models.DomainModel.Vacation&gt;" />
    public interface IProjectRepository : IRepository<Project>
    {
        void AddNewProjectGroups(Project? project, int[] groupIds, IRepository<Group> groupData);
    }
}
