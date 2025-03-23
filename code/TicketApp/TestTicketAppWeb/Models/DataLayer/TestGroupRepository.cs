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
    public class TestGroupRepository : IDisposable
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
            var groups = await _repository.GetAllAsync();

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
            var group = await _repository.GetAsync("G1");

            Assert.NotNull(group);
            Assert.Equal("Test Group", group.GroupName);
            Assert.Equal(2, group.Members.Count);
            Assert.Equal("Manager", group.Manager.UserName);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenIdIsInvalid()
        {
            var group = await _repository.GetAsync("InvalidID");

            Assert.Null(group);
        }

        [Fact]
        public async Task InsertAsync_ShouldAddGroupToDatabase()
        {
            var newGroup = new Group
            {
                Id = "G2",
                GroupName = "New Group",
                Description = "Another test group",
                ManagerId = "3",
                Members = new HashSet<TicketAppUser>()
            };

            await _repository.InsertAsync(newGroup);
            await _repository.SaveAsync();

            var addedGroup = await _repository.GetAsync("G2");
            Assert.NotNull(addedGroup);
            Assert.Equal("New Group", addedGroup.GroupName);
        }

        [Fact]
        public async Task SaveAsync_ShouldPersistChanges()
        {
            var group = new Group
            {
                Id = "G3",
                GroupName = "Save Group",
                Description = "Persist Test",
                ManagerId = "3",
                Members = new HashSet<TicketAppUser>()
            };

            await _repository.InsertAsync(group);
            await _repository.SaveAsync();

            var saved = await _repository.GetAsync("G3");
            Assert.NotNull(saved);
            Assert.Equal("Save Group", saved.GroupName);
        }

        [Fact]
        public async Task DeleteGroupAsync_ShouldRemoveGroupFromDatabase()
        {
            var group = await _repository.GetAsync("G1");
            Assert.NotNull(group);

            await _repository.DeleteGroupAsync(group);

            var deletedGroup = await _repository.GetAsync("G1");
            Assert.Null(deletedGroup);
        }

        [Fact]
        public async Task DeleteGroupAsync_ShouldRemoveGroupFromProjects()
        {
            var group = await _repository.GetAsync("G1");

            var project = new Project
            {
                Id = "P1",
                ProjectName = "Test Project",
                LeadId = "1",
                Groups = new List<Group> { group }
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            await _repository.DeleteGroupAsync(group);

            var updatedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == "P1");
            Assert.NotNull(updatedProject);
            Assert.Empty(updatedProject.Groups);
        }

        [Fact]
        public void AddNewGroupMembers_ShouldAddMembersToGroup()
        {
            var group = _context.Groups.First(g => g.Id == "G1");
            var user3 = new TicketAppUser { Id = "4", UserName = "User3" };
            _context.Users.Add(user3);
            _context.SaveChanges();

            var mockUserRepo = new Mock<IRepository<TicketAppUser>>();
            mockUserRepo.Setup(repo => repo.Get("4")).Returns(user3);

            _repository.AddNewGroupMembers(group, new[] { "4" }, mockUserRepo.Object);
            _context.SaveChanges();

            Assert.Contains(group.Members, u => u.Id == "4");
        }

        [Fact]
        public void AddNewGroupMembers_ShouldDoNothing_WhenGroupIsNull()
        {
            var mockUserRepo = new Mock<IRepository<TicketAppUser>>();

            _repository.AddNewGroupMembers(null, new[] { "1", "2" }, mockUserRepo.Object);

            Assert.True(true); 
        }

        [Fact]
        public void AddNewGroupMembers_ShouldIgnoreExistingMembers()
        {
            var group = _context.Groups.First(g => g.Id == "G1");
            var existingId = group.Members.First().Id;

            var mockUserRepo = new Mock<IRepository<TicketAppUser>>();
            mockUserRepo.Setup(r => r.Get(existingId)).Returns(group.Members.First());

            var initialCount = group.Members.Count;

            _repository.AddNewGroupMembers(group, new[] { existingId }, mockUserRepo.Object);
            _context.SaveChanges();

            Assert.Equal(initialCount, group.Members.Count);
        }

        [Fact]
        public async Task DeleteGroupAsync_ShouldDoNothing_WhenGroupIsNull()
        {
            await _repository.DeleteGroupAsync(null);
            Assert.True(true); 
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
