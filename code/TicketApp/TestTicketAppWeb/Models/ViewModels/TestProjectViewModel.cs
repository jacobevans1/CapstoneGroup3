using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

namespace TestTicketAppWeb.Models.ViewModels;

public class TestProjectViewModel
{
    [Fact]
    public void FilteredProjects_NoFilters_ReturnsAllProjects()
    {
        // Arrange
        var projects = new List<Project>
            {
                new Project { ProjectName = "Project1", Lead = new TicketAppUser { FirstName = "Lead1" } },
                new Project { ProjectName = "Project2", Lead = new TicketAppUser { FirstName = "Lead2" } }
            };
        var viewModel = new ProjectViewModel { Projects = projects };

        // Act
        var result = viewModel.FilteredProjects;

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void FilteredProjects_WithProjectNameFilter_ReturnsFilteredProjects()
    {
        // Arrange
        var projects = new List<Project>
            {
                new Project { ProjectName = "Project1", Lead = new TicketAppUser { FirstName = "Lead1" } },
                new Project { ProjectName = "Project2", Lead = new TicketAppUser { FirstName = "Lead2" } }
            };
        var viewModel = new ProjectViewModel { Projects = projects, SearchProjectName = "Project1" };

        // Act
        var result = viewModel.FilteredProjects;

        // Assert
        Assert.Single(result);
        Assert.Equal("Project1", result.First().ProjectName);
    }

    [Fact]
    public void FilteredProjects_WithProjectLeadFilter_ReturnsFilteredProjects()
    {
        // Arrange
        var projects = new List<Project>
            {
                new Project { ProjectName = "Project1", Lead = new TicketAppUser { FirstName = "Lead1" } },
                new Project { ProjectName = "Project2", Lead = new TicketAppUser { FirstName = "Lead2" } }
            };
        var viewModel = new ProjectViewModel { Projects = projects, SearchProjectLead = "Lead1" };

        // Act
        var result = viewModel.FilteredProjects;

        // Assert
        Assert.Single(result);
        Assert.Equal("Lead1 ", result.First().Lead?.FullName);
    }

    [Fact]
    public void IsCurrentUserProjectLeadForProject_CurrentUserIsLead_ReturnsTrue()
    {
        // Arrange
        var user = new TicketAppUser { Id = "1" };
        var project = new Project { LeadId = "1" };
        var viewModel = new ProjectViewModel { CurrentUser = user };

        // Act
        var result = viewModel.IsCurrentUserProjectLeadForProject(project);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCurrentUserProjectLeadForProject_CurrentUserIsNull_ReturnsFalse()
    {
        // Arrange
        var project = new Project { LeadId = "1" };
        var viewModel = new ProjectViewModel { CurrentUser = null };

        // Act
        var result = viewModel.IsCurrentUserProjectLeadForProject(project);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCurrentUserProjectLeadForProject_CurrentUserIsNotLead_ReturnsFalse()
    {
        // Arrange
        var user = new TicketAppUser { Id = "1" };
        var project = new Project { LeadId = "2" };
        var viewModel = new ProjectViewModel { CurrentUser = user };

        // Act
        var result = viewModel.IsCurrentUserProjectLeadForProject(project);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCurrentUserAGroupManagerInProject_CurrentUserIsManager_ReturnsTrue()
    {
        // Arrange
        var user = new TicketAppUser { Id = "1" };
        var group = new Group { ManagerId = "1" };
        var project = new Project { Id = "1" };
        var viewModel = new ProjectViewModel
        {
            CurrentUser = user,
            ProjectGroups = new Dictionary<Project, List<Group>> { { project, new List<Group> { group } } }
        };

        // Act
        var result = viewModel.IsCurrentUserAGroupManagerInProject(project);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCurrentUserAGroupManagerInProject_CurrentUserIsNull_ReturnsFalse()
    {
        // Arrange
        var group = new Group { ManagerId = "1" };
        var project = new Project { Id = "1" };
        var viewModel = new ProjectViewModel
        {
            CurrentUser = null,
            ProjectGroups = new Dictionary<Project, List<Group>> { { project, new List<Group> { group } } }
        };

        // Act
        var result = viewModel.IsCurrentUserAGroupManagerInProject(project);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCurrentUserAGroupManagerInProject_CurrentUserIsNotManager_ReturnsFalse()
    {
        // Arrange
        var user = new TicketAppUser { Id = "1" };
        var group = new Group { ManagerId = "2" };
        var project = new Project { Id = "1" };
        var viewModel = new ProjectViewModel
        {
            CurrentUser = user,
            ProjectGroups = new Dictionary<Project, List<Group>> { { project, new List<Group> { group } } }
        };

        // Act
        var result = viewModel.IsCurrentUserAGroupManagerInProject(project);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCurrentUserLeadForAnyGroup_CurrentUserIsLead_ReturnsTrue()
    {
        // Arrange
        var user = new TicketAppUser { Id = "1" };
        var group = new Group { ManagerId = "1" };
        var viewModel = new ProjectViewModel
        {
            CurrentUser = user,
            AvailableGroups = new List<Group> { group }
        };

        // Act
        var result = viewModel.IsCurrentUserLeadForAnyGroup();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCurrentUserLeadForAnyGroup_CurrentUserIsNull_ReturnsFalse()
    {
        // Arrange
        var group = new Group { ManagerId = "1" };
        var viewModel = new ProjectViewModel
        {
            CurrentUser = null,
            AvailableGroups = new List<Group> { group }
        };

        // Act
        var result = viewModel.IsCurrentUserLeadForAnyGroup();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCurrentUserLeadForAnyGroup_CurrentUserIsNotLead_ReturnsFalse()
    {
        // Arrange
        var user = new TicketAppUser { Id = "1" };
        var group = new Group { ManagerId = "2" };
        var viewModel = new ProjectViewModel
        {
            CurrentUser = user,
            AvailableGroups = new List<Group> { group }
        };

        // Act
        var result = viewModel.IsCurrentUserLeadForAnyGroup();

        // Assert
        Assert.False(result);
    }
}
