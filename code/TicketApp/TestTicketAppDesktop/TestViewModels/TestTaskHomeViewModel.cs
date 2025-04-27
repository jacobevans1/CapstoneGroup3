using Moq;
using TicketAppDesktop.Models;
using TicketAppDesktop.ViewModels;
using System.Collections.ObjectModel;


namespace TestTicketAppDesktop.ViewModels;

public class TaskHomeViewModelTests
{
	private readonly Mock<ITaskDAL> _mockDal;
	private readonly TaskHomeViewModel _vm;
	private readonly List<string> _changed = new();

	public TaskHomeViewModelTests()
	{
		UserSession.CurrentUserId = "u1";

		_mockDal = new Mock<ITaskDAL>();
		_vm = new TaskHomeViewModel(_mockDal.Object);
		_vm.PropertyChanged += (s, e) => _changed.Add(e.PropertyName!);
	}

	[Fact]
	public void Ctor_NullDAL_ThrowsArgumentNullException()
	{
		Assert.Throws<ArgumentNullException>(() => new TaskHomeViewModel(null!));
	}

	[Fact]
	public void Ctor_SetsFilters_AndDefaultSelectedFilter()
	{
		Assert.Equal(new[] { "Available", "My Tasks" }, _vm.Filters);
		Assert.Equal("My Tasks", _vm.SelectedFilter);
	}


	[Fact]
	public void Ctor_SetsFilters_DefaultSelectedFilter_AndFiresEvents()
	{
		Assert.Equal(new[] { "Available", "My Tasks" }, _vm.Filters);

		_changed.Clear();
		_vm.SelectedFilter = _vm.SelectedFilter;

		Assert.Contains("SelectedFilter", _changed);
		Assert.Contains("Tasks", _changed);
	}

	[Fact]
	public void RefreshTasks_WhenAvailable_UsesGetAvailableTasksForUserGroups()
	{
		var avail = new List<Ticket> { new Ticket { Id = "T1" } };
		_mockDal.Setup(d => d.GetAvailableTasksForUserGroups("u1")).Returns(avail);

		_changed.Clear();
		_vm.SelectedFilter = "Available";

		Assert.Equal(new ObservableCollection<Ticket>(avail), _vm.Tasks);
		_mockDal.Verify(d => d.GetAvailableTasksForUserGroups("u1"), Times.Once);
		_mockDal.Verify(d => d.GetTasksByAssignee(It.IsAny<string>()), Times.Never);

		Assert.Contains("SelectedFilter", _changed);
		Assert.Contains("Tasks", _changed);
	}

	[Fact]
	public void RefreshTasks_WhenMyTasks_UsesGetTasksByAssignee()
	{
		var mine = new List<Ticket> { new Ticket { Id = "X" }, new Ticket { Id = "Y" } };
		_mockDal.Setup(d => d.GetTasksByAssignee("u1")).Returns(mine);

		_changed.Clear();
		_vm.RefreshTasks();

		Assert.Equal(new ObservableCollection<Ticket>(mine), _vm.Tasks);
		_mockDal.Verify(d => d.GetTasksByAssignee("u1"), Times.Once);
		_mockDal.Verify(d => d.GetAvailableTasksForUserGroups(It.IsAny<string>()), Times.Never);

		Assert.Contains("Tasks", _changed);
	}

	[Fact]
	public void ReassigningSameFilter_StillInvokesRefreshTasks()
	{
		_changed.Clear();

		_mockDal.Invocations.Clear();

		var current = _vm.SelectedFilter;
		_vm.SelectedFilter = current;

		Assert.Contains("SelectedFilter", _changed);
		Assert.Contains("Tasks", _changed);
		_mockDal.Verify(d => d.GetTasksByAssignee("u1"), Times.Once);

	}
}
