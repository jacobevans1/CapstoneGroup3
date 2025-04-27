using Moq;

using System.Collections.ObjectModel;
using TicketAppDesktop.Models;
using TicketAppDesktop.ViewModels;

namespace TestTicketAppDesktop.ViewModels;

public class TestsTicketDetailViewModel
{
	private readonly Mock<ITicketDAL> _ticketDal;
	private readonly Mock<IStageDAL> _stageDal;
	private readonly Mock<ITicketHistoryDAL> _historyDal;
	private readonly Mock<IUsersDAL> _usersDal;
	private readonly TicketDetailViewModel _vm;
	private readonly List<string> _changedProps = new();

	public TestsTicketDetailViewModel()
	{
		_ticketDal = new Mock<ITicketDAL>();
		_stageDal = new Mock<IStageDAL>();
		_historyDal = new Mock<ITicketHistoryDAL>();
		_usersDal = new Mock<IUsersDAL>();

		// construct view‐model with mocked dependencies
		_vm = new TicketDetailViewModel(
			_ticketDal.Object,
			_stageDal.Object,
			_historyDal.Object,
			_usersDal.Object
		);

		// capture all INotifyPropertyChanged events
		_vm.PropertyChanged += (s, e) => _changedProps.Add(e.PropertyName!);
	}

	[Fact]
	public void Load_TicketNotFound_ThrowsInvalidOperation()
	{
		_ticketDal.Setup(d => d.GetTicketById("missing"))
				  .Returns((Ticket)null!);

		Assert.Throws<InvalidOperationException>(() => _vm.Load("missing"));
	}

	[Fact]
	public void Load_Valid_PopulatesAndRaisesAllEvents()
	{
		var ticket = new Ticket
		{
			Id = "t1",
			BoardId = "b1",
			Title = "T",
			Description = "D",
			AssignedTo = "u1",
			Stage = "s1"
		};
		var stages = new List<Stage>
		{
			new Stage { Id = "s1", Name = "StageName" }
		};
		var history = new List<TicketHistory>
		{
			new TicketHistory { Id = "h1", TicketId = "t1", ChangeDescription = "desc", ChangeDate = DateTime.Now }
		};

		_ticketDal.Setup(d => d.GetTicketById("t1")).Returns(ticket);
		_stageDal.Setup(d => d.GetStagesForBoard("b1")).Returns(stages);
		_historyDal.Setup(d => d.GetHistoryByTicketId("t1")).Returns(history);

		_vm.Load("t1");

		Assert.Equal(ticket, _vm.Ticket);
		Assert.Equal(new ObservableCollection<Stage>(stages), _vm.Stages);
		Assert.Equal(new ObservableCollection<TicketHistory>(history), _vm.History);

		// all seven property‐changed notifications
		Assert.Contains("Ticket", _changedProps);
		Assert.Contains("Stages", _changedProps);
		Assert.Contains("History", _changedProps);
		Assert.Contains("Title", _changedProps);
		Assert.Contains("Description", _changedProps);
		Assert.Contains("SelectedStageId", _changedProps);
		Assert.Contains("Assigned", _changedProps);
	}

	[Fact]
	public void TitleSetter_RaisesPropertyChanged()
	{
		_vm.Ticket = new Ticket();
		_changedProps.Clear();

		_vm.Title = "NewTitle";

		Assert.Contains("Title", _changedProps);
		Assert.Equal("NewTitle", _vm.Ticket!.Title);
	}

	[Fact]
	public void DescriptionSetter_RaisesPropertyChanged()
	{
		_vm.Ticket = new Ticket();
		_changedProps.Clear();

		_vm.Description = "NewDesc";

		Assert.Contains("Description", _changedProps);
		Assert.Equal("NewDesc", _vm.Ticket!.Description);
	}

	[Fact]
	public void SelectedStageIdSetter_RaisesPropertyChanged()
	{
		_vm.Ticket = new Ticket();
		_changedProps.Clear();

		_vm.SelectedStageId = "stageX";

		Assert.Contains("SelectedStageId", _changedProps);
		Assert.Equal("stageX", _vm.Ticket!.Stage);
	}

	[Fact]
	public void AssignedSetter_RaisesPropertyChanged()
	{
		_vm.Ticket = new Ticket();
		_changedProps.Clear();

		_vm.Assigned = true;

		Assert.Contains("Assigned", _changedProps);
		Assert.NotNull(_vm.Ticket!.AssignedTo);
	}

	[Fact]
	public void SaveChanges_TicketNotFound_ThrowsInvalidOperation()
	{
		_vm.Ticket = new Ticket { Id = "x" };
		_ticketDal.Setup(d => d.GetTicketById("x")).Returns((Ticket)null!);

		Assert.Throws<InvalidOperationException>(() => _vm.SaveChanges());
	}

	[Fact]
	public void SaveChanges_NoChanges_DoesNothing()
	{
		// Arrange
		var orig = new Ticket
		{
			Id = "t2",
			Title = "Same",
			Description = "SameDesc",
			AssignedTo = "u1",
			Stage = "s1"
		};

		_ticketDal.Setup(d => d.GetTicketById("t2")).Returns(orig);
		_ticketDal.Setup(d => d.UpdateTicket(orig)).Verifiable();
		_historyDal.Setup(d => d.SaveHistoryEntry(It.IsAny<TicketHistory>()))
				   .Verifiable("No history entries should be saved");
		_usersDal.Setup(d => d.GetFullName(It.IsAny<string>())).Returns("Alice");
		_stageDal.Setup(d => d.GetStagesForBoard(It.IsAny<string>()))
				 .Returns(new List<Stage>());

		_vm.Ticket = orig;

		// **ensure** History & Stages collections are non‐null
		_vm.History = new ObservableCollection<TicketHistory>();
		_vm.Stages = new ObservableCollection<Stage>();

		_changedProps.Clear();

		// Act
		_vm.SaveChanges();

		// Assert
		Assert.Empty(_vm.History);
		_ticketDal.Verify(d => d.UpdateTicket(orig), Times.Once);
		_historyDal.Verify(d => d.SaveHistoryEntry(It.IsAny<TicketHistory>()), Times.Never);
	}

	[Fact]
	public void SaveChanges_DescriptionChanged_LogsOnlyDescription()
	{
		var orig = new Ticket
		{
			Id = "t3",
			Title = "T",
			Description = "OldDesc",
			AssignedTo = "u1",
			Stage = "s1"
		};
		var updated = new Ticket
		{
			Id = "t3",
			Title = "T",
			Description = "NewDesc",
			AssignedTo = "u1",
			Stage = "s1"
		};

		_ticketDal.Setup(d => d.GetTicketById("t3")).Returns(orig);
		_ticketDal.Setup(d => d.UpdateTicket(updated)).Verifiable();
		var logged = new List<TicketHistory>();
		_historyDal.Setup(d => d.SaveHistoryEntry(It.IsAny<TicketHistory>()))
				   .Callback<TicketHistory>(h => logged.Add(h));
		_usersDal.Setup(d => d.GetFullName("u1")).Returns("Alice");
		_stageDal.Setup(d => d.GetStagesForBoard(It.IsAny<string>()))
				 .Returns(new List<Stage> { new Stage { Id = "s1", Name = "One" } });

		_vm.Ticket = updated;
		_vm.History = new ObservableCollection<TicketHistory>();
		_vm.Stages = new ObservableCollection<Stage>();

		_changedProps.Clear();
		_vm.SaveChanges();

		Assert.Single(logged);
		Assert.Equal("Description", logged[0].PropertyChanged);
		Assert.Contains("NewDesc", logged[0].NewValue);
	}

	[Fact]
	public void SaveChanges_AssignedToChanged_LogsAssignedTo()
	{
		var orig = new Ticket
		{
			Id = "t4",
			Title = "T",
			Description = "D",
			AssignedTo = null,
			Stage = "s1"
		};
		var updated = new Ticket
		{
			Id = "t4",
			Title = "T",
			Description = "D",
			AssignedTo = "u2",
			Stage = "s1"
		};

		_ticketDal.Setup(d => d.GetTicketById("t4")).Returns(orig);
		_ticketDal.Setup(d => d.UpdateTicket(updated)).Verifiable();
		var logged = new List<TicketHistory>();
		_historyDal.Setup(d => d.SaveHistoryEntry(It.IsAny<TicketHistory>()))
				   .Callback<TicketHistory>(h => logged.Add(h));
		_usersDal.Setup(d => d.GetFullName("u2")).Returns("Bob");
		_stageDal.Setup(d => d.GetStagesForBoard(It.IsAny<string>()))
				 .Returns(new List<Stage> { new Stage { Id = "s1", Name = "One" } });

		_vm.Ticket = updated;
		_vm.History = new ObservableCollection<TicketHistory>();
		_vm.Stages = new ObservableCollection<Stage>();

		_changedProps.Clear();
		_vm.SaveChanges();

		Assert.Single(logged);
		Assert.Equal("AssignedTo", logged[0].PropertyChanged);
		Assert.Equal("Bob", logged[0].NewValue);
	}

	[Fact]
	public void SaveChanges_StageChanged_LogsStageName()
	{
		var orig = new Ticket
		{
			Id = "t5",
			Title = "T",
			Description = "D",
			AssignedTo = "u1",
			Stage = "s1"
		};
		var updated = new Ticket
		{
			Id = "t5",
			Title = "T",
			Description = "D",
			AssignedTo = "u1",
			Stage = "s2"
		};

		_ticketDal.Setup(d => d.GetTicketById("t5")).Returns(orig);
		_ticketDal.Setup(d => d.UpdateTicket(updated)).Verifiable();
		var logged = new List<TicketHistory>();
		_historyDal.Setup(d => d.SaveHistoryEntry(It.IsAny<TicketHistory>()))
				   .Callback<TicketHistory>(h => logged.Add(h));
		_stageDal.Setup(d => d.GetStagesForBoard(It.IsAny<string>()))
				 .Returns(new List<Stage>
				 {
					 new Stage { Id = "s1", Name = "First" },
					 new Stage { Id = "s2", Name = "Second" }
				 });
		_usersDal.Setup(d => d.GetFullName("u1")).Returns("Alice");

		_vm.Ticket = updated;
		_vm.History = new ObservableCollection<TicketHistory>();
		_vm.Stages = new ObservableCollection<Stage>();

		_changedProps.Clear();
		_vm.SaveChanges();

		Assert.Single(logged);
		Assert.Equal("Stage", logged[0].PropertyChanged);
		Assert.Equal("Second", logged[0].NewValue);
	}

	[Fact]
	public void SaveChanges_MultipleChanges_LogsAll()
	{
		var orig = new Ticket
		{
			Id = "t6",
			Title = "Old",
			Description = "Desc1",
			AssignedTo = "u1",
			Stage = "s1"
		};
		var updated = new Ticket
		{
			Id = "t6",
			Title = "New",
			Description = "Desc2",
			AssignedTo = null,
			Stage = "s2"
		};

		_ticketDal.Setup(d => d.GetTicketById("t6")).Returns(orig);
		_ticketDal.Setup(d => d.UpdateTicket(updated)).Verifiable();
		var logged = new List<TicketHistory>();
		_historyDal.Setup(d => d.SaveHistoryEntry(It.IsAny<TicketHistory>()))
				   .Callback<TicketHistory>(h => logged.Add(h));
		_usersDal.Setup(d => d.GetFullName("u1")).Returns("Alice");
		_stageDal.Setup(d => d.GetStagesForBoard(It.IsAny<string>()))
				 .Returns(new List<Stage>
				 {
					 new Stage { Id = "s1", Name = "One" },
					 new Stage { Id = "s2", Name = "Two" }
				 });

		_vm.Ticket = updated;
		_vm.History = new ObservableCollection<TicketHistory>();
		_vm.Stages = new ObservableCollection<Stage>();

		_changedProps.Clear();
		_vm.SaveChanges();

		// Title, Description, AssignedTo, Stage = 4 entries
		Assert.Equal(4, logged.Count);
	}

	[Fact]
	public void SaveChanges_TitleChanged_LogsTitle()
	{
		// Arrange
		// make sure UserSession.CurrentUserId is set and stubbed
		UserSession.CurrentUserId = "uX";
		_usersDal.Setup(d => d.GetFullName("uX")).Returns("User X");
		// also stub the original AssignedTo lookup so AssignedTo switch never NREs
		_usersDal.Setup(d => d.GetFullName("u1")).Returns("ExistingUser");

		var orig = new Ticket
		{
			Id = "t100",
			Title = "OldTitle",
			Description = "Desc",
			AssignedTo = "u1",
			Stage = "s1"
		};
		var updated = new Ticket
		{
			Id = "t100",
			Title = "NewTitle",
			Description = "Desc",
			AssignedTo = "u1",
			Stage = "s1"
		};

		_ticketDal.Setup(d => d.GetTicketById("t100")).Returns(orig);
		_ticketDal.Setup(d => d.UpdateTicket(updated)).Verifiable();

		var logged = new List<TicketHistory>();
		_historyDal.Setup(d => d.SaveHistoryEntry(It.IsAny<TicketHistory>()))
				   .Callback<TicketHistory>(h => logged.Add(h));

		// need at least an empty stages list to avoid NRE in Stage branch
		_stageDal.Setup(d => d.GetStagesForBoard(It.IsAny<string>()))
				 .Returns(new List<Stage>());

		_vm.Ticket = updated;
		_vm.History = new ObservableCollection<TicketHistory>();
		_vm.Stages = new ObservableCollection<Stage>();
		_changedProps.Clear();

		// Act
		_vm.SaveChanges();

		// Assert
		Assert.Single(logged);
		Assert.Equal("Title", logged[0].PropertyChanged);
		Assert.Contains("updated Title from", logged[0].ChangeDescription);
		Assert.Contains("OldTitle", logged[0].ChangeDescription);
		Assert.Contains("NewTitle", logged[0].ChangeDescription);
	}

	[Fact]
	public void SaveChanges_AssignedToRemoved_LogsUnassigned()
	{
		// Arrange
		UserSession.CurrentUserId = "uY";
		_usersDal.Setup(d => d.GetFullName("uY")).Returns("User Y");
		// stub the original AssignedTo lookup
		_usersDal.Setup(d => d.GetFullName("u1")).Returns("Bob");

		var orig = new Ticket
		{
			Id = "t200",
			Title = "T",
			Description = "D",
			AssignedTo = "u1",
			Stage = "s1"
		};
		var updated = new Ticket
		{
			Id = "t200",
			Title = "T",
			Description = "D",
			AssignedTo = null,
			Stage = "s1"
		};

		_ticketDal.Setup(d => d.GetTicketById("t200")).Returns(orig);
		_ticketDal.Setup(d => d.UpdateTicket(updated)).Verifiable();

		var logged = new List<TicketHistory>();
		_historyDal.Setup(d => d.SaveHistoryEntry(It.IsAny<TicketHistory>()))
				   .Callback<TicketHistory>(h => logged.Add(h));

		_stageDal.Setup(d => d.GetStagesForBoard(It.IsAny<string>()))
				 .Returns(new List<Stage>());

		_vm.Ticket = updated;
		_vm.History = new ObservableCollection<TicketHistory>();
		_vm.Stages = new ObservableCollection<Stage>();
		_changedProps.Clear();

		// Act
		_vm.SaveChanges();

		// Assert
		Assert.Single(logged);
		Assert.Equal("AssignedTo", logged[0].PropertyChanged);
		Assert.Equal("Unassigned", logged[0].NewValue);
		Assert.Contains("changed assignee", logged[0].ChangeDescription);
	}

	[Fact]
	public void TitleGetter_ReturnsEmpty_WhenTicketNull()
	{
		var vm2 = new TicketDetailViewModel(
			_ticketDal.Object,
			_stageDal.Object,
			_historyDal.Object,
			_usersDal.Object);

		Assert.Equal("", vm2.Title);
	}

	[Fact]
	public void TitleGetter_ReturnsValue_WhenTicketHasTitle()
	{
		var vm2 = new TicketDetailViewModel(
			_ticketDal.Object,
			_stageDal.Object,
			_historyDal.Object,
			_usersDal.Object);

		vm2.Ticket = new Ticket { Title = "MyTitle" };
		Assert.Equal("MyTitle", vm2.Title);
	}

	[Fact]
	public void SelectedStageIdGetter_CoversNullAndValue()
	{
		var vm2 = new TicketDetailViewModel(
			_ticketDal.Object,
			_stageDal.Object,
			_historyDal.Object,
			_usersDal.Object);

		// Ticket null → null
		Assert.Null(vm2.SelectedStageId);

		// Ticket with Stage
		vm2.Ticket = new Ticket { Stage = "stage42" };
		Assert.Equal("stage42", vm2.SelectedStageId);
	}

	[Fact]
	public void AssignedGetter_CoversFalseAndTrue()
	{
		var vm2 = new TicketDetailViewModel(
			_ticketDal.Object,
			_stageDal.Object,
			_historyDal.Object,
			_usersDal.Object);

		// Ticket null → false
		Assert.False(vm2.Assigned);

		// Ticket with AssignedTo null → false
		vm2.Ticket = new Ticket { AssignedTo = null };
		Assert.False(vm2.Assigned);

		// Ticket with AssignedTo non‐null → true
		vm2.Ticket.AssignedTo = "someone";
		Assert.True(vm2.Assigned);
	}

	[Fact]
	public void OnPropertyChanged_InvokesSubscriber_WhenSubscribed()
	{
		var vm3 = new TicketDetailViewModel(
			_ticketDal.Object,
			_stageDal.Object,
			_historyDal.Object,
			_usersDal.Object);

		var seen = new List<string>();
		vm3.PropertyChanged += (s, e) => seen.Add(e.PropertyName!);

		// setting Ticket triggers OnPropertyChanged("Ticket"),("Title"),("Description"),("SelectedStageId"),("Assigned")
		vm3.Ticket = new Ticket();
		Assert.Contains("Ticket", seen);
		Assert.Contains("Title", seen);
		Assert.Contains("Description", seen);
		Assert.Contains("SelectedStageId", seen);
		Assert.Contains("Assigned", seen);
	}

	[Fact]
	public void OnPropertyChanged_NoSubscriber_DoesNotThrow()
	{
		var vm4 = new TicketDetailViewModel(
			_ticketDal.Object,
			_stageDal.Object,
			_historyDal.Object,
			_usersDal.Object);

		// No subscribers attached → setting a property should not throw
		vm4.Ticket = new Ticket();
	}
}
