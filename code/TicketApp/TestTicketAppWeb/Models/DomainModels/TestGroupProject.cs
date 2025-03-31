using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class TestGroupProject
	{
		[Fact]
		public void GroupProject_Properties_SetAndGetCorrectly()
		{
			// Arrange
			var groupProject = new GroupProject();
			var project = new Project { Id = "Project1", ProjectName = "Test Project" };
			var group = new Group { Id = "Group1", GroupName = "Test Group" };

			// Act
			groupProject.ProjectsId = "Project1";
			groupProject.GroupsId = "Group1";
			groupProject.Project = project;
			groupProject.Group = group;

			// Assert
			Assert.Equal("Project1", groupProject.ProjectsId);
			Assert.Equal("Group1", groupProject.GroupsId);
			Assert.Equal(project, groupProject.Project);
			Assert.Equal(group, groupProject.Group);
		}

		[Fact]
		public void GroupProject_DefaultConstructor_InitializesObject()
		{
			// Act
			var groupProject = new GroupProject();

			// Assert
			Assert.NotNull(groupProject);
		}
	}
}
