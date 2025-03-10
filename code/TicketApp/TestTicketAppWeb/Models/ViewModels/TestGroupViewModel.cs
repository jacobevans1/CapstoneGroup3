using System;
using System.Collections.Generic;
using System.Linq;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.Grid;
using TicketAppWeb.Models.ViewModels;
using Xunit;

namespace TestTicketAppWeb.Models.ViewModels
{
    public class TestGroupViewModel
    {
        [Fact]
        public void GroupViewModel_InitializesWithDefaults()
        {
            // Arrange & Act
            var viewModel = new GroupViewModel();

            // Assert
            Assert.Null(viewModel.CurrentUser);
            Assert.Null(viewModel.CurrentUserRole);
            Assert.NotNull(viewModel.Group);
            Assert.Empty(viewModel.AssignedUsers);
            Assert.Empty(viewModel.AvailableUsers);
            Assert.Empty(viewModel.AvailableGroupManagers);
            Assert.Null(viewModel.GroupManagerId);
            Assert.Null(viewModel.GroupManagerName);
            Assert.Null(viewModel.SearchGroupName);
            Assert.Null(viewModel.SearchGroupLead);
            Assert.Empty(viewModel.Groups);
            Assert.NotNull(viewModel.CurrentRoute);
            Assert.Equal(10, viewModel.SelectedPageSize);
            Assert.Null(viewModel.SearchTerm);
        }

        [Fact]
        public void GroupViewModel_CanSetProperties()
        {
            // Arrange
            var viewModel = new GroupViewModel();
            var user = new TicketAppUser { Id = "1", UserName = "testuser" };
            var users = new List<TicketAppUser> { user };
            var group = new Group { Id = "G1", GroupName = "Test Group" };
            var groups = new List<Group> { group };
            var route = new GroupGridData { PageNumber = 2 };

            // Act
            viewModel.CurrentUser = user;
            viewModel.CurrentUserRole = "Admin";
            viewModel.Group = group;
            viewModel.AssignedUsers = users;
            viewModel.AvailableUsers = users;
            viewModel.AvailableGroupManagers = users;
            viewModel.GroupManagerId = "1";
            viewModel.GroupManagerName = "testuser";
            viewModel.SearchGroupName = "Test";
            viewModel.SearchGroupLead = "Lead";
            viewModel.Groups = groups;
            viewModel.CurrentRoute = route;
            viewModel.SelectedPageSize = 20;
            viewModel.SearchTerm = "Example";

            // Assert
            Assert.Equal(user, viewModel.CurrentUser);
            Assert.Equal("Admin", viewModel.CurrentUserRole);
            Assert.Equal(group, viewModel.Group);
            Assert.Single(viewModel.AssignedUsers);
            Assert.Single(viewModel.AvailableUsers);
            Assert.Single(viewModel.AvailableGroupManagers);
            Assert.Equal("1", viewModel.GroupManagerId);
            Assert.Equal("testuser", viewModel.GroupManagerName);
            Assert.Equal("Test", viewModel.SearchGroupName);
            Assert.Equal("Lead", viewModel.SearchGroupLead);
            Assert.Single(viewModel.Groups);
            Assert.Equal(route, viewModel.CurrentRoute);
            Assert.Equal(20, viewModel.SelectedPageSize);
            Assert.Equal("Example", viewModel.SearchTerm);
        }

        [Fact]
        public void FilteredGroups_ShouldFilterCorrectly()
        {
            // Arrange
            var group1 = new Group { Id = "G1", GroupName = "Test Group A", Manager = new TicketAppUser { FirstName = "Alice", LastName = "Smith" } };
            var group2 = new Group { Id = "G2", GroupName = "Sample Group B", Manager = new TicketAppUser { FirstName = "Bob", LastName = "Johnson" } };
            var groups = new List<Group> { group1, group2 };

            var viewModel = new GroupViewModel
            {
                Groups = groups,
                SearchGroupName = "Test",
                SearchGroupLead = "Alice"
            };

            // Act
            var filteredGroups = viewModel.FilteredGroups.ToList();

            // Assert
            Assert.Single(filteredGroups);
            Assert.Equal("Test Group A", filteredGroups.First().GroupName);
        }

        [Fact]
        public void IsCurrentUserGroupManagerForGroup_ShouldReturnTrue_WhenUserIsManager()
        {
            // Arrange
            var user = new TicketAppUser { Id = "1" };
            var group = new Group { Id = "G1", ManagerId = "1" };
            var viewModel = new GroupViewModel { CurrentUser = user };

            // Act
            var result = viewModel.IsCurrentUserGroupManagerForGroup(group);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCurrentUserGroupManagerForGroup_ShouldReturnFalse_WhenUserIsNotManager()
        {
            // Arrange
            var user = new TicketAppUser { Id = "2" };
            var group = new Group { Id = "G1", ManagerId = "1" };
            var viewModel = new GroupViewModel { CurrentUser = user };

            // Act
            var result = viewModel.IsCurrentUserGroupManagerForGroup(group);

            // Assert
            Assert.False(result);
        }
    }
}
