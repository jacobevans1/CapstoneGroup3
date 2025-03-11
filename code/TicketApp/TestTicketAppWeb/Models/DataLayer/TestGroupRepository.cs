using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using Xunit;

namespace TestTicketAppWeb.Models.DataLayer
{
    public class TestGroupRepository
    {
        private readonly TicketAppContext _context;
        private readonly GroupRepository _repository;

        public TestGroupRepository()
        {
            var dbName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<TicketAppContext>()
                .UseInMemoryDatabase(dbName) 
                .Options;

            _context = new TicketAppContext(options);
            _context.Database.EnsureDeleted(); 
            _context.Database.EnsureCreated(); 

            _repository = new GroupRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var user1 = new TicketAppUser { Id = "1", UserName = "User1" };
            var user2 = new TicketAppUser { Id = "2", UserName = "User2" };
            var manager = new TicketAppUser { Id = "3", UserName = "Manager" };

            var group = new Group
            {
                Id = "G1",
                GroupName = "Test Group",
                Description = "This is a test group",
                ManagerId = "3",
                Manager = manager,
                Members = new HashSet<TicketAppUser> { user1, user2 }
            };

            _context.Users.AddRange(user1, user2, manager);
            _context.Groups.Add(group);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnGroups_WithMembersAndManager()
        {
            // Act
            var groups = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(groups);
            Assert.Single(groups);
            Assert.Equal("Test Group", groups.First().GroupName);
            Assert.Equal(2, groups.First().Members.Count);
            Assert.NotNull(groups.First().Manager);
            Assert.Equal("Manager", groups.First().Manager.UserName);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnGroup_WhenIdIsValid()
        {
            // Act
            var group = await _repository.GetAsync("G1");

            // Assert
            Assert.NotNull(group);
            Assert.Equal("Test Group", group.GroupName);
            Assert.Equal(2, group.Members.Count);
            Assert.Equal("Manager", group.Manager.UserName);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenIdIsInvalid()
        {
            // Act
            var group = await _repository.GetAsync("InvalidID");

            // Assert
            Assert.Null(group);
        }

        [Fact]
        public async Task InsertAsync_ShouldAddGroupToDatabase()
        {
            // Arrange
            var newGroup = new Group
            {
                Id = "G2",
                GroupName = "New Group",
                Description = "Another test group",
                ManagerId = "3",
                Members = new HashSet<TicketAppUser>()
            };

            // Act
            await _repository.InsertAsync(newGroup);
            await _repository.SaveAsync();

            // Assert
            var addedGroup = await _repository.GetAsync("G2");
            Assert.NotNull(addedGroup);
            Assert.Equal("New Group", addedGroup.GroupName);
        }

        [Fact]
        public async Task DeleteGroupAsync_ShouldRemoveGroupFromDatabase()
        {
            // Arrange
            var group = await _repository.GetAsync("G1");
            Assert.NotNull(group);

            // Act
            await _repository.DeleteGroupAsync(group);
            var deletedGroup = await _repository.GetAsync("G1");

            // Assert
            Assert.Null(deletedGroup);
        }

        [Fact]
        public async Task DeleteGroupAsync_ShouldRemoveGroupFromProjects()
        {
            // Arrange
            var project = new Project
            {
                Id = "P1",
                ProjectName = "Test Project",
                LeadId = "user1_Id",
                Groups = new List<Group> { await _repository.GetAsync("G1") }
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var group = await _repository.GetAsync("G1");
            Assert.NotNull(group);
            Assert.Single(project.Groups);

            // Act
            await _repository.DeleteGroupAsync(group);
            var updatedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == "P1");

            // Assert
            Assert.NotNull(updatedProject);
            Assert.Empty(updatedProject.Groups); 
        }

        [Fact]
        public void AddNewGroupMembers_ShouldAddMembersToGroup()
        {
            // Arrange
            var group = _context.Groups.First(g => g.Id == "G1");
            var user3 = new TicketAppUser { Id = "4", UserName = "User3" };
            _context.Users.Add(user3);
            _context.SaveChanges();

            var mockUserRepo = new Mock<IRepository<TicketAppUser>>();
            mockUserRepo.Setup(repo => repo.Get("4")).Returns(user3);

            // Act
            _repository.AddNewGroupMembers(group, new[] { "4" }, mockUserRepo.Object);
            _context.SaveChanges();

            // Assert
            Assert.Contains(group.Members, u => u.Id == "4");
        }

        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
