using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories;
using TicketAppWeb.Models.DomainModels;

namespace TestTicketAppWeb.Models.DataLayer;

public class TestTicketRepository : IDisposable
{
	private readonly TicketAppContext _context;
	private readonly TicketRepository _repo;

	public TestTicketRepository()
	{
		var opts = new DbContextOptionsBuilder<TicketAppContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;
		_context = new TicketAppContext(opts);
		_repo = new TicketRepository(_context);
	}

	public void Dispose() => _context.Dispose();

	[Fact]
	public void AddTicket_Null_ThrowsArgumentNullException()
	{
		Assert.Throws<ArgumentNullException>(() => _repo.AddTicket(null!));
	}

	[Fact]
	public void AddTicket_ValidTicket_SavesTicketAndHistory()
	{
		// Arrange: seed users and a stage
		var creator = new TicketAppUser { Id = "u1", FirstName = "Alice", LastName = "Karen" };
		var assignee = new TicketAppUser { Id = "u2", FirstName = "Bob" , LastName = "Quon" };
		var stage = new Stage { Id = "st1", Name = "Todo" };
		_context.Users.AddRange(creator, assignee);
		_context.Stages.Add(stage);
		_context.SaveChanges();

		var ticket = new Ticket
		{
			Id = "t1",
			Title = "Test Ticket",
			CreatedBy = "u1",
			AssignedTo = "u2",
			Stage = "st1",
			History = new List<TicketHistory>()
		};

		// Act
		_repo.AddTicket(ticket);

		// Assert: ticket was inserted
		var saved = _context.Tickets.Find("t1");
		Assert.NotNull(saved);

		// Assert: exactly one history entry, linked to ticket t1
		var hist = _context.TicketHistories.Where(h => h.TicketId == "t1").ToList();
		Assert.Single(hist);
		Assert.Contains("created", hist[0].ChangeDescription, StringComparison.OrdinalIgnoreCase);
		Assert.Contains("Initial Assignee: Bob Quon", hist[0].ChangeDescription);
		Assert.Contains("Initial Stage: Todo", hist[0].ChangeDescription);
	}

	[Fact]
	public void AddTicket_NoCreatorOrAssignee_UsesUnknownUserAndOmitsAssignee()
	{
		// Arrange: no users, no stage
		var ticket = new Ticket
		{
			Id = "t_no",
			Title = "NoOne",
			CreatedBy = "uX",
			AssignedTo = null,
			Stage = null,
			History = new List<TicketHistory>()
		};

		// Act
		_repo.AddTicket(ticket);

		// Assert
		var hist = _context.TicketHistories.Single(h => h.TicketId == "t_no");
		Assert.Contains("Unknown User created", hist.ChangeDescription);
		Assert.DoesNotContain("Initial Assignee:", hist.ChangeDescription);
		Assert.Contains("Initial Stage: Unspecified", hist.ChangeDescription);
	}

	[Fact]
	public void AddTicket_NoStageProvided_AppendsUnspecifiedStage()
	{
		// Arrange: seed only the creator
		var creator = new TicketAppUser { Id = "u_c", FirstName = "Creator", LastName = "Of Ticket" };
		_context.Users.Add(creator);
		_context.SaveChanges();

		var ticket = new Ticket
		{
			Id = "t2_no_stage",
			Title = "Ticket2",
			CreatedBy = "u_c",
			AssignedTo = null,
			Stage = null,
			History = new List<TicketHistory>()
		};

		// Act
		_repo.AddTicket(ticket);

		// Assert
		var hist = _context.TicketHistories.Single(h => h.TicketId == "t2_no_stage");
		Assert.Contains("Initial Stage: Unspecified", hist.ChangeDescription);
	}

	[Fact]
	public void UpdateTicket_UnchangedTitle_DoesNotErrorAndKeepsValue()
	{
		// Arrange
		var ticket = new Ticket
		{
			Id = "t_same",
			Title = "Title",
			Description = "Desc1",
			AssignedTo = "u1",
			Stage = "st1"
		};
		_context.Tickets.Add(ticket);
		_context.SaveChanges();

		var updated = new Ticket
		{
			Title = "Title", 
			Description = "Desc2",
			AssignedTo = "u2",
			Stage = "st2"
		};

		// Act
		_repo.UpdateTicket(ticket, updated);

		// Assert
		var saved = _context.Tickets.Find("t_same")!;
		Assert.Equal("Title", saved.Title);
		Assert.Equal("Desc2", saved.Description);
		Assert.Equal("u2", saved.AssignedTo);
		Assert.Equal("st2", saved.Stage);
	}

	[Fact]
	public void UpdateTicket_ChangesFieldsAndPersists()
	{
		// Arrange: a ticket already in the store
		var ticket = new Ticket
		{
			Id = "t2",
			Title = "Old",
			Description = "Desc1",
			AssignedTo = "u1",
			Stage = "st1"
		};
		_context.Tickets.Add(ticket);
		_context.SaveChanges();

		var updated = new Ticket
		{
			Title = "New",
			Description = "Desc2",
			AssignedTo = "u2",
			Stage = "st2"
		};

		// Act
		_repo.UpdateTicket(ticket, updated);

		// Assert
		var saved = _context.Tickets.Find("t2")!;
		Assert.Equal("New", saved.Title);
		Assert.Equal("Desc2", saved.Description);
		Assert.Equal("u2", saved.AssignedTo);
		Assert.Equal("st2", saved.Stage);
	}

	[Fact]
	public void DeleteTicket_Nonexistent_ThrowsArgumentNullException()
	{
		Assert.Throws<ArgumentNullException>(() => _repo.DeleteTicket("noSuchId"));
	}

	[Fact]
	public void DeleteTicket_Existing_RemovesTicket()
	{
		// Arrange
		var ticket = new Ticket { Id = "t3", Title = "WillDelete" };
		_context.Tickets.Add(ticket);
		_context.SaveChanges();

		// Act
		_repo.DeleteTicket("t3");

		// Assert
		Assert.Null(_context.Tickets.Find("t3"));
	}

	[Fact]
	public void GetTicketWithHistory_NoAssignee_DoesNotPopulateAssignedToUser()
	{
		// Arrange
		var user = new TicketAppUser { Id = "u1", FirstName = "User1", LastName = "Lastname" };
		var ticket = new Ticket
		{
			Id = "t_no_assign",
			Title = "NoAssign",
			CreatedBy = "u1",
			AssignedTo = null,
			Stage = null,
			History = new List<TicketHistory>()
		};
		_context.Users.Add(user);
		_context.Tickets.Add(ticket);
		_context.SaveChanges();

		// Act
		var result = _repo.GetTicketWithHistory("t_no_assign");

		// Assert
		Assert.NotNull(result);
		Assert.Null(result.AssignedToUser);
	}

	[Fact]
	public void GetTicketWithHistory_NotFound_ReturnsNull()
	{
		var result = _repo.GetTicketWithHistory("doesNotExist");
		Assert.Null(result);
	}

	[Fact]
	public void GetTicketWithHistory_ReturnsTicketHistoryAndAssignee()
	{
		// Arrange: users, ticket, and history
		var creator = new TicketAppUser { Id = "u1", FirstName = "Alice", LastName = "Karen" };
		var assignee = new TicketAppUser { Id = "u2", FirstName = "Bob" , LastName = "Quon" };
		var ticket = new Ticket
		{
			Id = "t4",
			Title = "HistTest",
			CreatedBy = "u1",
			AssignedTo = "u2",
			Stage = null,
			History = new List<TicketHistory>()
		};
		var hist = new TicketHistory
		{
			Id = "h1",
			TicketId = "t4",            
			PropertyChanged = "Created",
			OldValue = null,
			NewValue = "HistTest",
			ChangedByUserId = "u1",
			ChangeDate = DateTime.Now,
			ChangeDescription = "Desc"
		};

		_context.Users.AddRange(creator, assignee);
		_context.Tickets.Add(ticket);
		_context.TicketHistories.Add(hist);
		_context.SaveChanges();

		// Act
		var result = _repo.GetTicketWithHistory("t4");

		// Assert
		Assert.NotNull(result);
		Assert.Equal("t4", result.Id);

		// history loaded
		Assert.Single(result.History);
		Assert.Equal("Desc", result.History.First().ChangeDescription);

		// assigned‐to user populated
		Assert.NotNull(result.AssignedToUser);
		Assert.Equal("Bob Quon", result.AssignedToUser!.FullName);
	}
}