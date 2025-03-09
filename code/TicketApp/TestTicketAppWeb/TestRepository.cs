using Microsoft.EntityFrameworkCore;
using Moq;
using TicketAppWeb.Models.DataLayer.Reposetories;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories;

/// <summary>
/// Tests the reposetory class
/// Jabesi
/// 02/23/2025
/// </summary>
public class TestsRepository
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
    public void Count_ReturnsCorrectCount()
    {
        // Arrange
        var data = new List<Project>
        {
            new Project { Id = "project1Id", LeadId = "user1_id", ProjectName = "Project 1", Description = "Description 1" },
            new Project { Id = "project2Id", LeadId = "user2_id", ProjectName = "Project 2", Description = "Description 2" }
        };
        var context = CreateMockContext(data, "TestDatabase_Count");
        var repository = new Repository<Project>(context);

        // Act
        var count = repository.Count;

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void List_ReturnsAllEntities()
    {
        // Arrange
        var data = new List<Group>
        {
            new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
            new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
        };
        var context = CreateMockContext(data, "TestDatabase_List");
        var repository = new Repository<Group>(context);
        var options = new QueryOptions<Group>();

        // Act
        var result = repository.List(options);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void List_WithWhereClause_ReturnsFilteredEntities()
    {
        // Arrange
        var data = new List<Group>
        {
            new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
            new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" }
        };
        var context = CreateMockContext(data, "TestDatabase_ListWithWhere");
        var repository = new Repository<Group>(context);
        var options = new QueryOptions<Group> { Where = g => g.ManagerId == "manager1Id" };

        // Act
        var result = repository.List(options);

        // Assert
        Assert.Single(result);
        Assert.Equal("group1Id", result.First().Id);
    }

    [Fact]
    public void List_WithOrderBy_ReturnsOrderedEntities()
    {
        // Arrange
        var data = new List<Group>
        {
            new Group { Id = "group1Id", GroupName = "Group B", ManagerId = "manager1Id" },
            new Group { Id = "group2Id", GroupName = "Group A", ManagerId = "manager2Id" }
        };
        var context = CreateMockContext(data, "TestDatabase_ListWithOrderBy");
        var repository = new Repository<Group>(context);
        var options = new QueryOptions<Group> { OrderBy = g => g.GroupName! };

        // Act
        var result = repository.List(options);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("group2Id", result.First().Id);
    }

    [Fact]
    public void List_WithOrderByDescending_ReturnsOrderedEntities()
    {
        // Arrange
        var data = new List<Group>
    {
        new Group { Id = "group1Id", GroupName = "Group A", ManagerId = "manager1Id" },
        new Group { Id = "group2Id", GroupName = "Group B", ManagerId = "manager2Id" }
    };
        var context = CreateMockContext(data, "TestDatabase_ListWithOrderByDescending");
        var repository = new Repository<Group>(context);
        var options = new QueryOptions<Group> { OrderBy = g => g.GroupName!, OrderByDirection = "desc" };

        // Act
        var result = repository.List(options);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("group2Id", result.First().Id);
    }

    [Fact]
    public void List_WithPaging_ReturnsPagedEntities()
    {
        // Arrange
        var data = new List<Group>
        {
            new Group { Id = "group1Id", GroupName = "Group 1", ManagerId = "manager1Id" },
            new Group { Id = "group2Id", GroupName = "Group 2", ManagerId = "manager2Id" },
            new Group { Id = "group3Id", GroupName = "Group 3", ManagerId = "manager3Id" }
        };
        var context = CreateMockContext(data, "TestDatabase_ListWithPaging");
        var repository = new Repository<Group>(context);
        var options = new QueryOptions<Group> { PageNumber = 1, PageSize = 2 };

        // Act
        var result = repository.List(options);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void Get_ById_ReturnsEntity()
    {
        // Arrange
        var data = new List<TicketAppUser>
        {
            new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" },
            new TicketAppUser { Id = "user2Id", FirstName = "Jane", LastName = "Doe" }
        };
        var context = CreateMockContext(data, "TestDatabase_GetById");
        var repository = new Repository<TicketAppUser>(context);

        // Act
        var result = repository.Get("user1Id");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user1Id", result.Id);
    }

    [Fact]
    public void Get_WithQueryOptions_ReturnsEntity()
    {
        // Arrange
        var data = new List<TicketAppUser>
        {
            new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" },
            new TicketAppUser { Id = "user2Id", FirstName = "Jane", LastName = "Doe" }
        };
        var context = CreateMockContext(data, "TestDatabase_GetWithQueryOptions");
        var repository = new Repository<TicketAppUser>(context);
        var options = new QueryOptions<TicketAppUser> { Where = u => u.FirstName == "John" };

        // Act
        var result = repository.Get(options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user1Id", result.Id);
    }

    [Fact]
    public void Insert_AddsEntity()
    {
        // Arrange
        var data = new List<Project>();
        var context = CreateMockContext(data, "TestDatabase_Insert");
        var repository = new Repository<Project>(context);
        var entity = new Project { Id = "project1Id", LeadId = "user1_id", ProjectName = "New Project", Description = "Project Description" };

        // Act
        repository.Insert(entity);
        repository.Save();

        // Assert
        Assert.Single(context.Projects);
        Assert.Contains(entity, context.Projects);
    }

    [Fact]
    public void Update_UpdatesEntity()
    {
        // Arrange
        var data = new List<Group> { new Group { Id = "group1Id", GroupName = "Old Name", ManagerId = "manager1Id" } };
        var context = CreateMockContext(data, "TestDatabase_Update");
        var repository = new Repository<Group>(context);
        var entity = new Group { Id = "group1Id", GroupName = "Updated", ManagerId = "manager1Id" };

        context.Entry(data.First()).State = EntityState.Detached;

        // Act
        repository.Update(entity);
        repository.Save();

        // Assert
        Assert.Equal("Updated", context.Groups.AsNoTracking().First().GroupName);
    }



    [Fact]

    public void Delete_RemovesEntity()
    {
        // Arrange
        var data = new List<TicketAppUser> { new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" } };
        var context = CreateMockContext(data, "TestDatabase_Delete");
        var repository = new Repository<TicketAppUser>(context);
        var entity = new TicketAppUser { Id = "user1Id", FirstName = "John", LastName = "Doe" };

        var existingEntity = context.Users.Find("user1Id");

        if (existingEntity != null)
        {
            context.Entry(existingEntity).State = EntityState.Detached;
        }

        // Act
        try
        {
            repository.Delete(entity);
            repository.Save();
        }

        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            var clientValues = (TicketAppUser)entry.Entity;
            var databaseEntry = entry.GetDatabaseValues();

            if (databaseEntry == null)
            {
                throw new DbUpdateConcurrencyException("The entity has been deleted by another user.");
            }

            var databaseValues = (TicketAppUser)databaseEntry.ToObject();

            entry.OriginalValues.SetValues(databaseValues);

            repository.Delete(entity);
            repository.Save();

        }

        // Assert
        Assert.Empty(context.Users);
    }
}