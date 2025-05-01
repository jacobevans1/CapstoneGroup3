using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using TicketAppDesktop.Models;
using TicketAppDesktop.ViewModels;
using Xunit;

namespace TestTicketAppDesktop.ViewModels
{
	public class TaskHomeViewModelTests
	{
		private readonly Mock<ITaskDAL> _mockDal;
		private readonly TaskHomeViewModel _vm;
		private readonly List<string> _changed = new();

		public TaskHomeViewModelTests()
		{
			// ensure a current user ID is set for each test
			UserSession.CurrentUserId = "u1";

			_mockDal = new Mock<ITaskDAL>();
			_vm = new TaskHomeViewModel(_mockDal.Object);
			_vm.PropertyChanged += (s, e) => _changed.Add(e.PropertyName!);
		}

		[Fact]
		public void Ctor_NullDAL_ThrowsArgumentNullException()
			=> Assert.Throws<ArgumentNullException>(() => new TaskHomeViewModel(null!));

		[Fact]
		public void Ctor_SetsFilters_AndDefaultSelectedFilter()
		{
			Assert.Equal(new[] { "Available", "My Tasks" }, _vm.Filters);

			// reading the getter *does* fire events & DAL, so test it separately:
			_changed.Clear();
			_mockDal.Invocations.Clear();

			var value = _vm.SelectedFilter; // getter
			Assert.Equal("My Tasks", value);

			Assert.Contains("SelectedFilter", _changed);
			Assert.Contains("Tasks", _changed);
			_mockDal.Verify(d => d.GetTasksByAssignee("u1"), Times.Once);
		}

		[Fact]
		public void SettingSameFilter_DoesNothing()
		{
			// clear out any events or DAL calls from ctor/getter
			_changed.Clear();
			_mockDal.Invocations.Clear();

			// assign the same literal value
			_vm.SelectedFilter = "My Tasks";

			Assert.Empty(_changed);
			_mockDal.VerifyNoOtherCalls();
		}

		[Fact]
		public void RefreshTasks_WhenAvailable_UsesGetAvailableTasksForUserGroups()
		{
			// arrange
			var avail = new List<Ticket> { new Ticket { Id = "T1" } };
			_mockDal.Setup(d => d.GetAvailableTasksForUserGroups("u1")).Returns(avail);

			// switch to Available
			_vm.SelectedFilter = "Available";

			// clear the events & DAL calls from that switch
			_changed.Clear();
			_mockDal.Invocations.Clear();

			// act
			_vm.RefreshTasks();

			// assert
			Assert.Equal(new ObservableCollection<Ticket>(avail), _vm.Tasks);
			_mockDal.Verify(d => d.GetAvailableTasksForUserGroups("u1"), Times.Once);
			_mockDal.Verify(d => d.GetTasksByAssignee(It.IsAny<string>()), Times.Never);

			Assert.Contains("Tasks", _changed);
		}

		[Fact]
		public void RefreshTasks_WhenMyTasks_UsesGetTasksByAssignee()
		{
			// arrange
			var mine = new List<Ticket>
			{
				new Ticket { Id = "X" },
				new Ticket { Id = "Y" }
			};
			_mockDal.Setup(d => d.GetTasksByAssignee("u1")).Returns(mine);

			// clear any state
			_changed.Clear();
			_mockDal.Invocations.Clear();

			// act
			_vm.RefreshTasks();

			// assert
			Assert.Equal(new ObservableCollection<Ticket>(mine), _vm.Tasks);
			_mockDal.Verify(d => d.GetTasksByAssignee("u1"), Times.Once);
			_mockDal.Verify(d => d.GetAvailableTasksForUserGroups(It.IsAny<string>()), Times.Never);

			Assert.Contains("Tasks", _changed);
		}
	}
}
