using System.Collections.Generic;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;
using Xunit;

namespace TestTicketAppWeb.Models.ViewModels
{
    public class TestDeleteGroupViewModel
    {
        [Fact]
        public void DeleteGroupViewModel_InitializesWithDefaults()
        {
            // Arrange
            var model = new DeleteGroupViewModel();

            // Assert
            Assert.Null(model.GroupId);
            Assert.Null(model.GroupName);
            Assert.Null(model.ManagerId);
            Assert.Null(model.ManagerName);
            Assert.NotNull(model.AffectedProjects);
            Assert.Empty(model.AffectedProjects);
            Assert.Null(model.NewLeadId);
            Assert.NotNull(model.AvailableUsers);
            Assert.Empty(model.AvailableUsers);
        }

        [Fact]
        public void DeleteGroupViewModel_CanSetAllProperties()
        {
            // Arrange
            var project1 = new Project { Id = "P1", ProjectName = "Alpha" };
            var project2 = new Project { Id = "P2", ProjectName = "Beta" };
            var user1 = new TicketAppUser { Id = "U1", UserName = "admin" };
            var user2 = new TicketAppUser { Id = "U2", UserName = "manager" };

            var model = new DeleteGroupViewModel
            {
                GroupId = "G123",
                GroupName = "Dev Team",
                ManagerId = "M456",
                ManagerName = "Jordan Bell",
                NewLeadId = "U2",
                AffectedProjects = new List<Project> { project1, project2 },
                AvailableUsers = new List<TicketAppUser> { user1, user2 }
            };

            // Assert
            Assert.Equal("G123", model.GroupId);
            Assert.Equal("Dev Team", model.GroupName);
            Assert.Equal("M456", model.ManagerId);
            Assert.Equal("Jordan Bell", model.ManagerName);
            Assert.Equal("U2", model.NewLeadId);
            Assert.Equal(2, model.AffectedProjects.Count);
            Assert.Equal(2, model.AvailableUsers.Count);
        }

        [Fact]
        public void DeleteGroupViewModel_AllowsEmptyProjectList()
        {
            var model = new DeleteGroupViewModel
            {
                GroupId = "G1",
                GroupName = "Test Group",
                ManagerId = "M1",
                ManagerName = "Sam",
                AffectedProjects = new List<Project>(), // explicitly set to empty
                AvailableUsers = new List<TicketAppUser>() // also explicitly set to empty
            };

            Assert.Empty(model.AffectedProjects);
            Assert.Empty(model.AvailableUsers);
        }

        [Fact]
        public void DeleteGroupViewModel_AllowsNullNewLeadId()
        {
            var model = new DeleteGroupViewModel
            {
                GroupId = "G1",
                GroupName = "Group X",
                ManagerId = "M123",
                ManagerName = "Yusuf",
                NewLeadId = null
            };

            Assert.Null(model.NewLeadId);
        }
    }
}
