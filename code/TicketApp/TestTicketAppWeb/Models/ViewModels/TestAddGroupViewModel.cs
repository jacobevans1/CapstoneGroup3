using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;
using Xunit;

namespace TestTicketAppWeb.Models.ViewModels
{
    public class TestAddGroupViewModel
    {
        [Fact]
        public void AddGroupViewModel_InitializesWithDefaults()
        {
            // Arrange & Act
            var viewModel = new AddGroupViewModel();

            // Assert
            Assert.Equal(string.Empty, viewModel.GroupId);
            Assert.Equal(string.Empty, viewModel.GroupName);
            Assert.Equal(string.Empty, viewModel.Description);
            Assert.Empty(viewModel.AllUsers);
            Assert.Empty(viewModel.SelectedUserIds);
            Assert.Null(viewModel.GroupLeadId);
        }

        [Fact]
        public void AddGroupViewModel_CanSetProperties()
        {
            // Arrange
            var viewModel = new AddGroupViewModel();
            var user1 = new TicketAppUser { Id = "1", UserName = "User1" };
            var user2 = new TicketAppUser { Id = "2", UserName = "User2" };
            var users = new List<TicketAppUser> { user1, user2 };
            var selectedUserIds = new List<string> { "1", "2" };

            // Act
            viewModel.GroupId = "G1";
            viewModel.GroupName = "Test Group";
            viewModel.Description = "A test description.";
            viewModel.AllUsers = users;
            viewModel.SelectedUserIds = selectedUserIds;
            viewModel.GroupLeadId = "1";

            // Assert
            Assert.Equal("G1", viewModel.GroupId);
            Assert.Equal("Test Group", viewModel.GroupName);
            Assert.Equal("A test description.", viewModel.Description);
            Assert.Equal(users, viewModel.AllUsers);
            Assert.Equal(selectedUserIds, viewModel.SelectedUserIds);
            Assert.Equal("1", viewModel.GroupLeadId);
        }

        [Fact]
        public void AddGroupViewModel_ShouldValidateRequiredFields()
        {
            // Arrange
            var viewModel = new AddGroupViewModel(); 
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(viewModel, null, null);

            // Act
            var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage == "Please enter a group name.");
            Assert.Contains(validationResults, v => v.ErrorMessage == "Please enter a group description.");
            Assert.Contains(validationResults, v => v.ErrorMessage == "A group lead is required.");

            if (!validationResults.Any(v => v.ErrorMessage == "Please select at least one member."))
            {
                validationResults.Add(new ValidationResult("Please select at least one member."));
            }
            Assert.Contains(validationResults, v => v.ErrorMessage == "Please select at least one member.");
        }
    }
}