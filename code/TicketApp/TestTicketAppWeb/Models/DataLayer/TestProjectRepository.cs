using Microsoft.EntityFrameworkCore;
using Moq;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Reposetories;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DataLayer;

/// <summary>
/// Tests the project repository
/// Jabesi Abwe
/// 03/08/2025
/// </summary>
public class ProjectRepositoryTests
{
	private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> elements) where T : class
	{
		var queryable = elements.AsQueryable();
		var mockSet = new Mock<DbSet<T>>();
		mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
		mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
		mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
		mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
		return mockSet;
	}

	private TicketAppContext CreateMockContext<T>(List<T> data, string dbName) where T : class
	{
		var options = new DbContextOptionsBuilder<TicketAppContext>()
			.UseInMemoryDatabase(databaseName: dbName)
			.Options;
		var context = new TicketAppContext(options);
		var mockSet = CreateMockDbSet(data);
		context.Set<T>().AddRange(data);
		context.SaveChanges();
		return context;
	}

	[Fact]
	public async Task AddProjectAsync_NewProject_ShouldCreateProject()
	{
		// Arrange
		var project = new Project
		{
			Id = "newId",
			ProjectName = "New Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId"
		};

		var groupIds = new List<string> { "group1Id", "group2Id" };

		var groups = new List<Group>
		{
			new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
			new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
		};

		var context = CreateMockContext(groups, "TestDatabase_NewProject");
		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: true);

		// Assert - Check that the new project is created
		var addedProject = await context.Projects
			.Include(p => p.Groups)
			.FirstOrDefaultAsync(p => p.ProjectName == "New Project");

		Assert.NotNull(addedProject);
		Assert.Equal(2, addedProject.Groups.Count);
	}

	[Fact]
	public async Task AddProjectAsync_ExistingProject_NoGroupsToAddOrRemove()
	{
		// Arrange
		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId",
			Groups = new List<Group>
			{
				new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
				new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
			}
		};

		var groupIds = new List<string> { "group1Id", "group2Id" };

		var context = CreateMockContext(new List<Group> { project.Groups.First(), project.Groups.Last() }, "TestDatabase_NoGroupsToAddOrRemove");
		context.Projects.Add(project);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: true);

		// Assert
		var updatedProject = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "existingId");
		Assert.NotNull(updatedProject);
		Assert.Equal(2, updatedProject.Groups.Count);
		Assert.Contains(updatedProject.Groups, g => g.Id == "group1Id");
		Assert.Contains(updatedProject.Groups, g => g.Id == "group2Id");
	}

	[Fact]
	public async Task AddProjectAsync_ExistingProject_RemoveAllGroups()
	{
		// Arrange
		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId",
			Groups = new List<Group>
			{
				new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
				new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
			}
		};

		var groupIds = new List<string>();

		var context = CreateMockContext(new List<Group> { project.Groups.First(), project.Groups.Last() }, "TestDatabase_RemoveAllGroups");
		context.Projects.Add(project);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: true);

		// Assert
		var updatedProject = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "existingId");
		Assert.NotNull(updatedProject);
		Assert.Empty(updatedProject.Groups);
	}

	[Fact]
	public async Task AddProjectAsync_NewProject_NoGroups()
	{
		// Arrange
		var project = new Project
		{
			Id = "newId",
			ProjectName = "New Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId"
		};

		var groupIds = new List<string>();

		var context = CreateMockContext(new List<Group>(), "TestDatabase_NewProject_NoGroups");
		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: true);

		// Assert
		var addedProject = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.ProjectName == "New Project");
		Assert.NotNull(addedProject);
		Assert.Empty(addedProject.Groups);
	}

	[Fact]
	public async Task AddProjectAsync_ExistingProject_AddNewGroup()
	{
		// Arrange
		var existingGroup = new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" };
		var newGroup = new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" };

		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId",
			Groups = new List<Group> { existingGroup }
		};

		var groupIds = new List<string> { "group1Id", "group2Id" };

		var context = CreateMockContext(new List<Group> { existingGroup, newGroup }, "TestDatabase_AddNewGroup");
		context.Projects.Add(project);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: true);

		// Assert
		var updatedProject = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "existingId");
		Assert.NotNull(updatedProject);
		Assert.Equal(2, updatedProject.Groups.Count);
		Assert.Contains(updatedProject.Groups, g => g.Id == "group1Id");
		Assert.Contains(updatedProject.Groups, g => g.Id == "group2Id");
	}

	[Fact]
	public async Task AddProjectAsync_ExistingProject_ShouldUpdateProject()
	{
		// Arrange
		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId"
		};

		var groupIds = new List<string> { "group1Id", "group2Id" };

		var groups = new List<Group>
		{
			new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
			new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
		};

		var context = CreateMockContext(groups, "TestDatabase_ExistingProject");
		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: true);

		// Assert - Check that the groups have been added to the existing project
		var existingProject = await context.Projects
			.Include(p => p.Groups)
			.FirstOrDefaultAsync(p => p.ProjectName == "Existing Project");

		Assert.NotNull(existingProject);
		Assert.Equal(2, existingProject.Groups.Count);
	}

	[Fact]
	public async Task AddProjectAsync_NonAdmin_ShouldOnlyAddManagedGroups()
	{
		// Arrange
		var userId = "manager1Id";
		var group1 = new Group { Id = "group1Id", ManagerId = userId };
		var group2 = new Group { Id = "group2Id", ManagerId = "manager2Id" };

		var context = CreateMockContext(new List<Group> { group1, group2 }, "TestDatabase_NonAdmin");
		var repository = new ProjectRepository(context);

		var newProject = new Project
		{
			Id = "newProjectId",
			ProjectName = "New Project",
			Description = "New Project Description",
			LeadId = "lead1Id",
			CreatedById = userId
		};

		var groupIds = new List<string> { "group1Id", "group2Id" };
		var isAdmin = false;

		// Act
		await repository.AddProjectAsync(newProject, groupIds, isAdmin);

		// Assert
		var addedProject = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "newProjectId");
		Assert.NotNull(addedProject);
		Assert.Single(addedProject.Groups);
		Assert.Contains(group1, addedProject.Groups);
	}

	[Fact]
	public async Task AddProjectAsync_GroupsRequiringApproval_ShouldCreateApprovalRequests()
	{
		// Arrange
		var project = new Project
		{
			Id = "newId",
			ProjectName = "New Project",
			LeadId = "lead1Id",
			CreatedById = "manager1Id"
		};

		var groupIds = new List<string> { "group2Id" };

		var groups = new List<Group>
		{
			new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
			new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
		};

		var context = CreateMockContext(groups, "TestDatabase_Approval");
		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: false);

		// Assert - Check that an approval request was created for group2
		var approvalRequest = await context.GroupApprovalRequests
			.FirstOrDefaultAsync(r => r.ProjectId == project.Id && r.GroupId == "group2Id");

		Assert.NotNull(approvalRequest);
		Assert.Equal("Pending", approvalRequest.Status);
	}

	[Fact]
	public async Task AddProjectAsync_ExistingGroup_ShouldNotAddGroupAgain()
	{
		// Arrange
		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId"
		};

		var groupIds = new List<string> { "group1Id" };

		var groups = new List<Group>
		{
			new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
			new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
		};

		var context = CreateMockContext(groups, "TestDatabase_ExistingGroup");
		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: true);

		// Assert - Ensure only one group is associated
		var existingProject = await context.Projects
			.Include(p => p.Groups)
			.FirstOrDefaultAsync(p => p.ProjectName == "Existing Project");

		Assert.NotNull(existingProject);
		Assert.Single(existingProject.Groups);
	}

	[Fact]
	public async Task AddProjectAsync_ExistingProject_ShouldAddAndRemoveGroupsCorrectly()
	{
		// Arrange
		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId"
		};

		var groupIds = new List<string> { "group1Id", "group3Id" };

		var groups = new List<Group>
		{
			new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
			new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" },
			new Group { Id = "group3Id", GroupName = "Group 3", ManagerId = "manager3Id" }
		};

		var context = CreateMockContext(groups, "TestDatabase_AddRemoveGroups");
		var repository = new ProjectRepository(context);

		// Act
		await repository.AddProjectAsync(project, groupIds, isAdmin: true);

		// Assert - Check that group1Id is still associated with the project
		var existingProject = await context.Projects
			.Include(p => p.Groups)
			.FirstOrDefaultAsync(p => p.ProjectName == "Existing Project");

		Assert.NotNull(existingProject);
		Assert.Equal(2, existingProject.Groups.Count);
		Assert.Contains(existingProject.Groups, g => g.Id == "group1Id");
		Assert.Contains(existingProject.Groups, g => g.Id == "group3Id");
		Assert.DoesNotContain(existingProject.Groups, g => g.Id == "group2Id");
	}


	[Fact]
	public async Task UpdateProjectAsync_ExistingProject_NoGroups_ShouldUpdateWithoutGroups()
	{
		// Arrange
		var context = CreateMockContext(new List<Project>
		{
			new Project { Id = "existingId", LeadId = "lead1Id", ProjectName = "Existing Project" }
		}, "TestDatabase_UpdateNoGroups");

		var repository = new ProjectRepository(context);
		var updatedProject = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project Updated",
			LeadId = "lead1Id",
			CreatedById = "creatorId"
		};

		var groupIds = new List<string>();
		var isAdmin = true;

		// Act
		await repository.UpdateProjectAsync(updatedProject, groupIds, isAdmin);

		// Assert
		var project = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "existingId");
		Assert.NotNull(project);
		Assert.Equal("Existing Project Updated", project.ProjectName);
		Assert.Empty(project.Groups);
	}

	[Fact]
	public async Task UpdateProjectAsync_ProjectNotFound_ShouldThrowKeyNotFoundException()
	{
		// Arrange
		var context = CreateMockContext(new List<Project>(), "TestDatabase_UpdateProjectNotFound");
		var repository = new ProjectRepository(context);
		var updatedProject = new Project
		{
			Id = "nonExistentProjectId",
			ProjectName = "Non Existent Project",
			Description = "Some Description",
			LeadId = "lead1Id",
			CreatedById = "adminId"
		};

		var groupIds = new List<string> { "group1Id" };
		var isAdmin = true;

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() =>
			repository.UpdateProjectAsync(updatedProject, groupIds, isAdmin));
	}

	[Fact]

	public async Task UpdateProjectAsync_ExistingProject_AddNewGroup()
	{
		// Arrange
		var existingGroup = new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" };

		var newGroup = new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" };

		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId",
			Groups = new List<Group> { existingGroup }
		};

		var groupIds = new List<string> { "group1Id", "group2Id" };

		var context = CreateMockContext(new List<Group> { existingGroup, newGroup }, "TestDatabase_UpdateAddNewGroup");

		context.Projects.Add(project);

		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		var updatedProject = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project Updated",
			LeadId = "lead1Id",
			CreatedById = "creatorId"
		};

		// Act

		await repository.UpdateProjectAsync(updatedProject, groupIds, isAdmin: true);

		// Assert
		var updatedProjectFromDb = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "existingId");

		Assert.NotNull(updatedProjectFromDb);
		Assert.Equal("Existing Project Updated", updatedProjectFromDb.ProjectName);
		Assert.Equal(2, updatedProjectFromDb.Groups.Count);
		Assert.Contains(updatedProjectFromDb.Groups, g => g.Id == "group1Id");
		Assert.Contains(updatedProjectFromDb.Groups, g => g.Id == "group2Id");
	}

	[Fact]
	public async Task UpdateProjectAsync_ExistingProject_RemoveGroup()
	{
		// Arrange
		var group1 = new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" };
		var group2 = new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" };

		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "creatorId",
			Groups = new List<Group> { group1, group2 }
		};

		var groupIds = new List<string> { "group1Id" };

		var context = CreateMockContext(new List<Group> { group1, group2 }, "TestDatabase_UpdateRemoveGroup");
		context.Projects.Add(project);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		var updatedProject = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project Updated",
			LeadId = "lead1Id",
			CreatedById = "creatorId"
		};

		// Act
		await repository.UpdateProjectAsync(updatedProject, groupIds, isAdmin: true);

		// Assert
		var updatedProjectFromDb = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "existingId");
		Assert.NotNull(updatedProjectFromDb);
		Assert.Equal("Existing Project Updated", updatedProjectFromDb.ProjectName);
		Assert.Single(updatedProjectFromDb.Groups);
		Assert.Contains(updatedProjectFromDb.Groups, g => g.Id == "group1Id");
	}

	[Fact]
	public async Task UpdateProjectAsync_NonAdmin_ShouldOnlyAddManagedGroups()
	{
		// Arrange
		var userId = "manager1Id";
		var group1 = new Group { Id = "group1Id", ManagerId = userId };
		var group2 = new Group { Id = "group2Id", ManagerId = "manager2Id" };

		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = userId,
			Groups = new List<Group> { group1 }
		};

		var groupIds = new List<string> { "group1Id", "group2Id" };

		var context = CreateMockContext(new List<Group> { group1, group2 }, "TestDatabase_UpdateNonAdmin");
		context.Projects.Add(project);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		var updatedProject = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project Updated",
			LeadId = "lead1Id",
			CreatedById = userId
		};

		// Act
		await repository.UpdateProjectAsync(updatedProject, groupIds, isAdmin: false);

		// Assert
		var updatedProjectFromDb = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "existingId");
		Assert.NotNull(updatedProjectFromDb);
		Assert.Equal("Existing Project Updated", updatedProjectFromDb.ProjectName);
		Assert.Single(updatedProjectFromDb.Groups);
		Assert.Contains(updatedProjectFromDb.Groups, g => g.Id == "group1Id");
	}

	[Fact]
	public async Task UpdateProjectAsync_GroupsRequiringApproval_ShouldCreateApprovalRequests()
	{
		// Arrange
		var group1 = new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" };
		var group2 = new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" };

		var project = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project",
			LeadId = "lead1Id",
			CreatedById = "manager1Id",
			Groups = new List<Group> { group1 }
		};

		var groupIds = new List<string> { "group2Id" };

		var context = CreateMockContext(new List<Group> { group1, group2 }, "TestDatabase_UpdateApproval");
		context.Projects.Add(project);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		var updatedProject = new Project
		{
			Id = "existingId",
			ProjectName = "Existing Project Updated",
			LeadId = "lead1Id",
			CreatedById = "manager1Id"
		};

		// Act
		await repository.UpdateProjectAsync(updatedProject, groupIds, isAdmin: false);

		// Assert
		var approvalRequest = await context.GroupApprovalRequests
			.FirstOrDefaultAsync(r => r.ProjectId == updatedProject.Id && r.GroupId == "group2Id");

		Assert.NotNull(approvalRequest);
		Assert.Equal("Pending", approvalRequest.Status);
	}
	[Fact]
	public async Task GetFilteredProjectsAndGroups_FilterByProjectName_ShouldReturnMatchingProjects()
	{
		// Arrange
		var user1 = new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" };
		var project1 = new Project { Id = "project1Id", ProjectName = "Test Project A", LeadId = "user1Id" };
		var project2 = new Project { Id = "project2Id", ProjectName = "Another Project B", LeadId = "user1Id" };

		var context = CreateMockContext(new List<TicketAppUser> { user1 }, "TestDatabase_FilterByProjectName");
		context.Projects.AddRange(project1, project2);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetFilteredProjectsAndGroups("Test Project", null);

		// Assert
		Assert.NotNull(result);
		Assert.Single(result);
		Assert.Contains(result.Keys, p => p.ProjectName == "Test Project A");
	}

	[Fact]
	public async Task GetFilteredProjectsAndGroups_FilterByLead_ShouldReturnMatchingProjects()
	{
		// Arrange
		var user1 = new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" };
		var user2 = new TicketAppUser { Id = "user2Id", FirstName = "Jane", LastName = "Smith" };
		var project1 = new Project { Id = "project1Id", ProjectName = "Test Project A", LeadId = "user1Id" };
		var project2 = new Project { Id = "project2Id", ProjectName = "Another Project B", LeadId = "user2Id" };

		var context = CreateMockContext(new List<TicketAppUser> { user1, user2 }, "TestDatabase_FilterByLead");
		context.Projects.AddRange(project1, project2);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetFilteredProjectsAndGroups(null, "John Doe");

		// Assert
		Assert.NotNull(result);
		Assert.Single(result);
		Assert.Contains(result.Keys, p => p.LeadId == "user1Id");
	}

	[Fact]
	public async Task GetFilteredProjectsAndGroups_ShouldReturnProjectsWithGroups()
	{
		// Arrange
		var user = new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" };
		var project = new Project { Id = "project1Id", ProjectName = "Test Project", LeadId = "user1Id" };
		var group = new Group { Id = "group1Id", GroupName = "Test Group", ManagerId = "manager1Id" };

		project.Groups = new List<Group> { group };

		var context = CreateMockContext(new List<TicketAppUser> { user }, "TestDatabase_ProjectsWithGroups");
		context.Projects.Add(project);
		context.Groups.Add(group);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetFilteredProjectsAndGroups(null, null);

		// Assert
		Assert.NotNull(result);
		Assert.Single(result);
		Assert.Contains(group, result[project]);
	}

	[Fact]
	public async Task GetFilteredProjectsAndGroups_ShouldPopulateLeadProperty()
	{
		// Arrange
		var user = new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" };
		var project = new Project { Id = "project1Id", ProjectName = "Test Project", LeadId = "user1Id" };

		var context = CreateMockContext(new List<TicketAppUser> { user }, "TestDatabase_PopulateLead");
		context.Projects.Add(project);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetFilteredProjectsAndGroups(null, null);

		// Assert
		Assert.NotNull(result);
		Assert.Single(result);
		Assert.NotNull(result.Keys.First().Lead);
		Assert.Equal("John Doe", result.Keys.First().Lead!.FullName);
	}

	[Fact]
	public async Task GetFilteredProjectsAndGroups_NoMatchingFilters_ShouldReturnEmpty()
	{
		// Arrange
		var user = new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" };
		var project = new Project { Id = "project1Id", ProjectName = "Test Project", LeadId = "user1Id" };

		var context = CreateMockContext(new List<TicketAppUser> { user }, "TestDatabase_NoMatchingFilters");
		context.Projects.Add(project);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetFilteredProjectsAndGroups("Nonexistent Project", "Nonexistent Lead");

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public async Task ApproveGroupForProjectAsync_ShouldApproveRequest()
	{
		// Arrange
		var project = new Project { Id = "project1Id", ProjectName = "Test Project", LeadId = "lead1Id", Groups = new List<Group>() };
		var group = new Group { Id = "group1Id", GroupName = "Test Group", ManagerId = "managerId" };
		var request = new GroupApprovalRequest
		{
			Id = "request1Id",
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Pending"
		};

		var context = CreateMockContext(new List<Group> { group }, "TestDatabase_ApproveGroup");
		context.Projects.Add(project);
		context.GroupApprovalRequests.Add(request);
		context.SaveChanges();

		var repository = new ProjectRepository(context);

		// Act
		await repository.ApproveGroupForProjectAsync("project1Id", "group1Id");

		// Assert
		var updatedRequest = await context.GroupApprovalRequests.FirstOrDefaultAsync(r => r.Id == "request1Id");
		Assert.NotNull(updatedRequest);
		Assert.Equal("Approved", updatedRequest.Status);

		var updatedProject = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "project1Id");
		Assert.NotNull(updatedProject);
		Assert.Contains(updatedProject.Groups, g => g.Id == "group1Id");
	}

	[Fact]
	public async Task ApproveGroupForProjectAsync_NoMatchingRequest_ShouldThrowException()
	{
		// Arrange
		var context = CreateMockContext(new List<Group>(), "TestDatabase_NoMatchingRequest");
		context.SaveChanges();

		var repository = new ProjectRepository(context);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<Exception>(() => repository.ApproveGroupForProjectAsync("project1Id", "group1Id"));
		Assert.StartsWith("Error approving group for project:", exception.Message);
	}

	[Fact]
	public async Task ApproveGroupForProjectAsync_ProjectNotFound_ShouldApproveWithoutAddingGroup()
	{
		// Arrange
		var group = new Group { Id = "group1Id", GroupName = "Test Group", ManagerId = "managerId" };
		var request = new GroupApprovalRequest
		{
			Id = "request1Id",
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Pending"
		};

		var context = CreateMockContext(new List<Group> { group }, "TestDatabase_ProjectNotFound");
		context.GroupApprovalRequests.Add(request);
		context.SaveChanges();

		var repository = new ProjectRepository(context);

		// Act
		await repository.ApproveGroupForProjectAsync("project1Id", "group1Id");

		// Assert
		var updatedRequest = await context.GroupApprovalRequests.FirstOrDefaultAsync(r => r.Id == "request1Id");
		Assert.NotNull(updatedRequest);
		Assert.Equal("Approved", updatedRequest.Status);
	}

	[Fact]
	public async Task ApproveGroupForProjectAsync_GroupNotFound_ShouldApproveWithoutAddingGroup()
	{
		// Arrange
		var project = new Project { Id = "project1Id", ProjectName = "Test Project", LeadId = "lead1Id", Groups = new List<Group>() };
		var request = new GroupApprovalRequest
		{
			Id = "request1Id",
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Pending"
		};

		var context = CreateMockContext(new List<Project> { project }, "TestDatabase_GroupNotFound");
		context.GroupApprovalRequests.Add(request);
		context.SaveChanges();

		var repository = new ProjectRepository(context);

		// Act
		await repository.ApproveGroupForProjectAsync("project1Id", "group1Id");

		// Assert
		var updatedRequest = await context.GroupApprovalRequests.FirstOrDefaultAsync(r => r.Id == "request1Id");
		Assert.NotNull(updatedRequest);
		Assert.Equal("Approved", updatedRequest.Status);

		var updatedProject = await context.Projects.Include(p => p.Groups).FirstOrDefaultAsync(p => p.Id == "project1Id");
		Assert.NotNull(updatedProject);
		Assert.Empty(updatedProject.Groups);
	}

	[Fact]
	public async Task RejectGroupForProjectAsync_ShouldRejectGroup()
	{
		// Arrange
		var project = new Project { Id = "project1Id", ProjectName = "Test Project" };
		var group = new Group { Id = "group1Id", GroupName = "Test Group", ManagerId = "managerId" };
		var request = new GroupApprovalRequest
		{
			Id = "request1Id",
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Pending"
		};

		var context = CreateMockContext(new List<Group> { group }, "TestDatabase_RejectGroup");
		context.GroupApprovalRequests.Add(request);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		await repository.RejectGroupForProjectAsync("project1Id", "group1Id");

		// Assert
		var updatedRequest = await context.GroupApprovalRequests.FirstOrDefaultAsync(r => r.Id == "request1Id");
		Assert.NotNull(updatedRequest);
		Assert.Equal("Rejected", updatedRequest.Status);
	}

	[Fact]
	public async Task RejectGroupForProjectAsync_NoPendingRequest_ShouldThrowException()
	{
		// Arrange
		var project = new Project { Id = "project1Id", ProjectName = "Test Project" };
		var group = new Group { Id = "group1Id", GroupName = "Test Group", ManagerId = "managerId" };

		var request = new GroupApprovalRequest
		{
			Id = "request1Id",
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Approved"
		};

		var context = CreateMockContext(new List<Group> { group }, "TestDatabase_NoPendingRequest");
		context.GroupApprovalRequests.Add(request);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<Exception>(() => repository.RejectGroupForProjectAsync("project1Id", "group1Id"));
		Assert.Contains("No pending approval request found.", exception.Message);
	}

	[Fact]
	public async Task RejectGroupForProjectAsync_NoMatchingRequest_ShouldThrowException()
	{
		// Arrange
		var project = new Project { Id = "project1Id", ProjectName = "Test Project" };
		var group = new Group { Id = "group1Id", GroupName = "Test Group", ManagerId = "managerId" };

		var request = new GroupApprovalRequest
		{
			Id = "request1Id",
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Pending"
		};

		var context = CreateMockContext(new List<Group> { group }, "TestDatabase_NoMatchingRequest");
		context.GroupApprovalRequests.Add(request);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<Exception>(() => repository.RejectGroupForProjectAsync("project1Id", "group2Id"));
		Assert.Contains("No pending approval request found.", exception.Message);
	}

	[Fact]
	public async Task GetPendingGroupApprovalRequestsAsync_ManagerHasPendingRequests_ShouldReturnRequests()
	{
		// Arrange
		var managerId = "manager1Id";
		var group1 = new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = managerId };
		var group2 = new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = managerId };

		var project1 = new Project { Id = "project1Id", LeadId = "user1_Id", ProjectName = "Project 1" };
		var project2 = new Project { Id = "project2Id", LeadId = "user2_Id", ProjectName = "Project 2" };

		var approvalRequest1 = new GroupApprovalRequest
		{
			Id = Guid.NewGuid().ToString(),
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Pending",
			Project = project1,
			Group = group1
		};

		var approvalRequest2 = new GroupApprovalRequest
		{
			Id = Guid.NewGuid().ToString(),
			ProjectId = "project2Id",
			GroupId = "group2Id",
			Status = "Pending",
			Project = project2,
			Group = group2
		};

		var context = CreateMockContext(new List<Group> { group1, group2 }, "TestDatabase_ManagerHasPendingRequests");
		context.GroupApprovalRequests.AddRange(approvalRequest1, approvalRequest2);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetPendingGroupApprovalRequestsAsync(managerId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.Count);
		Assert.Contains(result, r => r.GroupId == "group1Id" && r.Status == "Pending");
		Assert.Contains(result, r => r.GroupId == "group2Id" && r.Status == "Pending");
	}


	[Fact]
	public async Task GetPendingGroupApprovalRequestsAsync_ManagerHasNoPendingRequests_ShouldReturnEmptyList()
	{
		// Arrange
		var managerId = "manager1Id";
		var group1 = new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = managerId };
		var group2 = new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = managerId };

		var project1 = new Project { Id = "project1Id", LeadId = "user1_Id", ProjectName = "Project 1" };
		var project2 = new Project { Id = "project2Id", LeadId = "user2_Id", ProjectName = "Project 2" };

		var approvalRequest1 = new GroupApprovalRequest
		{
			Id = Guid.NewGuid().ToString(),
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Approved",
			Project = project1,
			Group = group1
		};

		var approvalRequest2 = new GroupApprovalRequest
		{
			Id = Guid.NewGuid().ToString(),
			ProjectId = "project2Id",
			GroupId = "group2Id",
			Status = "Rejected",
			Project = project2,
			Group = group2
		};

		var context = CreateMockContext(new List<Group> { group1, group2 }, "TestDatabase_ManagerHasNoPendingRequests");
		context.GroupApprovalRequests.AddRange(approvalRequest1, approvalRequest2);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetPendingGroupApprovalRequestsAsync(managerId);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetPendingGroupApprovalRequestsAsync_ManagerDoesNotManageAnyGroup_ShouldReturnEmptyList()
	{
		// Arrange
		var managerId = "manager1Id";
		var group1 = new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager2Id" };
		var group2 = new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" };

		var project1 = new Project { Id = "project1Id", LeadId = "user1_Id", ProjectName = "Project 1" };
		var project2 = new Project { Id = "project2Id", LeadId = "user2_Id", ProjectName = "Project 2" };

		var approvalRequest1 = new GroupApprovalRequest
		{
			Id = Guid.NewGuid().ToString(),
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Pending",
			Project = project1,
			Group = group1
		};

		var approvalRequest2 = new GroupApprovalRequest
		{
			Id = Guid.NewGuid().ToString(),
			ProjectId = "project2Id",
			GroupId = "group2Id",
			Status = "Pending",
			Project = project2,
			Group = group2
		};

		var context = CreateMockContext(new List<Group> { group1, group2 }, "TestDatabase_ManagerDoesNotManageAnyGroup");
		context.GroupApprovalRequests.AddRange(approvalRequest1, approvalRequest2);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetPendingGroupApprovalRequestsAsync(managerId);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetPendingGroupApprovalRequestsAsync_ManagerHasRequestsForOnlyTheirGroups_ShouldReturnFilteredRequests()
	{
		// Arrange
		var managerId = "manager1Id";
		var group1 = new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = managerId };
		var group2 = new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" };

		var project1 = new Project { Id = "project1Id", LeadId = "user1_Id", ProjectName = "Project 1" };
		var project2 = new Project { Id = "project2Id", LeadId = "user2_Id", ProjectName = "Project 2" };

		var approvalRequest1 = new GroupApprovalRequest
		{
			Id = Guid.NewGuid().ToString(),
			ProjectId = "project1Id",
			GroupId = "group1Id",
			Status = "Pending",
			Project = project1,
			Group = group1
		};

		var approvalRequest2 = new GroupApprovalRequest
		{
			Id = Guid.NewGuid().ToString(),
			ProjectId = "project2Id",
			GroupId = "group2Id",
			Status = "Pending",
			Project = project2,
			Group = group2
		};

		var context = CreateMockContext(new List<Group> { group1, group2 }, "TestDatabase_ManagerHasRequestsForTheirGroups");
		context.GroupApprovalRequests.AddRange(approvalRequest1, approvalRequest2);
		await context.SaveChangesAsync();

		var repository = new ProjectRepository(context);

		// Act
		var result = await repository.GetPendingGroupApprovalRequestsAsync(managerId);

		// Assert
		Assert.NotNull(result);
		Assert.Single(result);
		Assert.Contains(result, r => r.GroupId == "group1Id");
		Assert.DoesNotContain(result, r => r.GroupId == "group2Id");
	}
}