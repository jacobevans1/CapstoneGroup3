using Microsoft.EntityFrameworkCore;
using Moq;
using TicketAppWeb.Models.DataLayer.Reposetories;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories;


/// <summary>
/// Tests the Project reposetory class
/// Jabesi Abwe
/// 02/23/2025
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
    public void AddNewProjectGroups_RemovesExistingGroupsAndAddsNewGroups()
    {

        // Arrange
       var existingGroups = new List<Group>
    {
        new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
        new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
    };

        var newGroups = new List<Group>
    {
        new Group { Id = "group3Id", GroupName = "Group 3", ManagerId = "manager3Id" },
        new Group { Id = "group4Id", GroupName = "Group 4", ManagerId = "manager4Id" }
    };

        var project = new Project { Id = "project1Id", ProjectName = "Project 1", Description = "Description 1", Groups = existingGroups };
        var context = CreateMockContext(newGroups, "TestDatabase_AddNewProjectGroups");
        var groupData = new Repository<Group>(context);
        var repository = new ProjectRepository(context);
        var groupIds = newGroups.Select(g => g.Id).ToArray();

        // Act
        repository.AddNewProjectGroups(project, groupIds!, groupData);

        // Assert
        Assert.Equal(newGroups.Count, project.Groups.Count);
        Assert.All(newGroups, g => Assert.Contains(g, project.Groups));
    }

    [Fact]
    public void AddNewProjectGroups_DoesNotAddInvalidGroups()
    {
        // Arrange
        var existingGroups = new List<Group>
    {
        new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
        new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
    };
        var newGroups = new List<Group>
    {
        new Group { Id = "group3Id", GroupName = "Group 3", ManagerId = "manager3Id" }
    };

        var project = new Project { Id = "project1Id", ProjectName = "Project 1", Description = "Description 1", Groups = existingGroups };
        var context = CreateMockContext(newGroups, "TestDatabase_AddNewProjectGroups_Invalid");
        var groupData = new Repository<Group>(context);
        var repository = new ProjectRepository(context);
        var groupIds = new[] { "group3Id", "invalidGroupId" };

        // Act
        repository.AddNewProjectGroups(project, groupIds, groupData);

        // Assert
        Assert.Single(project.Groups);
        Assert.Contains(newGroups.First(), project.Groups);
    }
}