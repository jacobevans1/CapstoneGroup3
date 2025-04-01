using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace TestTicketAppWeb.Models.ViewModels
{
    public class TestGroupViewModel
    {
        [Fact]
        public void GroupViewModel_InitializesWithDefaults()
        {
            var viewModel = new GroupViewModel();

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
            Assert.Null(viewModel.SearchTerm);
        }

        [Fact]
        public void GroupViewModel_CanSetProperties()
        {
            var viewModel = new GroupViewModel();
            var user = new TicketAppUser { Id = "1", UserName = "testuser" };
            var users = new List<TicketAppUser> { user };
            var group = new Group { Id = "G1", GroupName = "Test Group" };
            var groups = new List<Group> { group };

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
            viewModel.SearchTerm = "Example";

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
            Assert.Equal("Example", viewModel.SearchTerm);
        }

        [Fact]
        public void FilteredGroups_ShouldReturnAll_WhenNoFiltersApplied()
        {
            var group1 = new Group { Id = "G1", GroupName = "Alpha", Manager = new TicketAppUser { FirstName = "Jane", LastName = "Doe" } };
            var group2 = new Group { Id = "G2", GroupName = "Beta", Manager = new TicketAppUser { FirstName = "John", LastName = "Smith" } };

            var viewModel = new GroupViewModel
            {
                Groups = new List<Group> { group1, group2 },
                SearchGroupName = null,
                SearchGroupLead = null
            };

            var filtered = viewModel.FilteredGroups.ToList();

            Assert.Equal(2, filtered.Count);
            Assert.Contains(group1, filtered);
            Assert.Contains(group2, filtered);
        }

        [Fact]
        public void FilteredGroups_ShouldFilterByNameOnly()
        {
            var group1 = new Group { Id = "G1", GroupName = "Development Team" };
            var group2 = new Group { Id = "G2", GroupName = "Marketing Squad" };

            var viewModel = new GroupViewModel
            {
                Groups = new List<Group> { group1, group2 },
                SearchGroupName = "Dev",
                SearchGroupLead = null
            };

            var filtered = viewModel.FilteredGroups.ToList();

            Assert.Single(filtered);
            Assert.Contains(group1, filtered);
        }

        [Fact]
        public void FilteredGroups_ShouldFilterByLeadOnly()
        {
            var manager1 = new TicketAppUser { FirstName = "Samuel", LastName = "Jackson" };
            var manager2 = new TicketAppUser { FirstName = "Lana", LastName = "DelRay" };

            var group1 = new Group { Id = "G1", GroupName = "Ops", Manager = manager1 };
            var group2 = new Group { Id = "G2", GroupName = "QA", Manager = manager2 };

            var viewModel = new GroupViewModel
            {
                Groups = new List<Group> { group1, group2 },
                SearchGroupLead = "Samuel"
            };

            var filtered = viewModel.FilteredGroups.ToList();

            Assert.Single(filtered);
            Assert.Equal(group1, filtered.First());
        }

        [Fact]
        public void FilteredGroups_ShouldFilterByNameAndLead()
        {
            var group1 = new Group
            {
                Id = "G1",
                GroupName = "Test Group A",
                Manager = new TicketAppUser { FirstName = "Alice", LastName = "Smith" }
            };
            var group2 = new Group
            {
                Id = "G2",
                GroupName = "Sample Group B",
                Manager = new TicketAppUser { FirstName = "Bob", LastName = "Johnson" }
            };
            var groups = new List<Group> { group1, group2 };

            var viewModel = new GroupViewModel
            {
                Groups = groups,
                SearchGroupName = "Test",
                SearchGroupLead = "Alice"
            };

            var filteredGroups = viewModel.FilteredGroups.ToList();

            Assert.Single(filteredGroups);
            Assert.Equal("Test Group A", filteredGroups.First().GroupName);
        }

        [Fact]
        public void IsCurrentUserGroupManagerForGroup_ShouldReturnTrue_WhenUserIsManager()
        {
            var user = new TicketAppUser { Id = "1" };
            var group = new Group { Id = "G1", ManagerId = "1" };
            var viewModel = new GroupViewModel { CurrentUser = user };

            var result = viewModel.IsCurrentUserGroupManagerForGroup(group);

            Assert.True(result);
        }

        [Fact]
        public void IsCurrentUserGroupManagerForGroup_ShouldReturnFalse_WhenUserIsNotManager()
        {
            var user = new TicketAppUser { Id = "2" };
            var group = new Group { Id = "G1", ManagerId = "1" };
            var viewModel = new GroupViewModel { CurrentUser = user };

            var result = viewModel.IsCurrentUserGroupManagerForGroup(group);

            Assert.False(result);
        }

        [Fact]
        public void IsCurrentUserGroupManagerForGroup_ShouldReturnFalse_WhenCurrentUserIsNull()
        {
            var group = new Group { Id = "G1", ManagerId = "1" };
            var viewModel = new GroupViewModel { CurrentUser = null };

            var result = viewModel.IsCurrentUserGroupManagerForGroup(group);

            Assert.False(result);
        }

        [Fact]
        public void FilteredGroups_ShouldBeCaseInsensitive()
        {
            var group = new Group
            {
                Id = "G1",
                GroupName = "Engineering Team",
                Manager = new TicketAppUser { FirstName = "Casey", LastName = "Turner" }
            };

            var viewModel = new GroupViewModel
            {
                Groups = new List<Group> { group },
                SearchGroupName = "engineering",
                SearchGroupLead = "casey"
            };

            var filtered = viewModel.FilteredGroups.ToList();
            Assert.Single(filtered);
            Assert.Equal("Engineering Team", filtered[0].GroupName);
        }

        [Fact]
        public void FilteredGroups_ManagerIsNull_ShouldNotThrow()
        {
            var group = new Group { Id = "G1", GroupName = "Orphan Group", Manager = null };

            var viewModel = new GroupViewModel
            {
                Groups = new List<Group> { group },
                SearchGroupLead = "any"
            };

            var filtered = viewModel.FilteredGroups.ToList();
            Assert.Empty(filtered); // Should not crash and just return nothing
        }

        [Fact]
        public void GroupViewModel_SelectedUserIds_DefaultsToEmptyArray()
        {
            var viewModel = new GroupViewModel();
            Assert.NotNull(viewModel.SelectedUserIds);
            Assert.Empty(viewModel.SelectedUserIds);
        }



    }
}
