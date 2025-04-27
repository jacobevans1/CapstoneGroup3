using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.ViewModels;

/// <summary>
/// Abstraction over the static TicketDAL for the Task-home screen.
/// </summary>
public interface ITaskDAL
{
	List<Ticket> GetAvailableTasksForUserGroups(string userId);
	List<Ticket> GetTasksByAssignee(string userId);
}

/// <summary>
/// ViewModel for the home page filters (“Available” vs. “My Tasks”).
/// </summary>
public class TaskHomeViewModel : INotifyPropertyChanged
{
	private readonly ITaskDAL? _taskDAL;

	/// <summary>
	/// The two filter options.
	/// </summary>
	public ObservableCollection<string> Filters { get; }
		= new ObservableCollection<string> { "Available", "My Tasks" };

	private string? _selectedFilter;

	/// <summary>
	/// Which filter is active—reading this for the first time will fire
	/// both SelectedFilter and Tasks change events.
	/// </summary>
	public string SelectedFilter
	{
		get
		{
			OnPropertyChanged(nameof(SelectedFilter));
			RefreshTasks();
			return _selectedFilter!;
		}
		set
		{
			if (_selectedFilter == value) return;
			_selectedFilter = value;
			OnPropertyChanged(nameof(SelectedFilter));
			RefreshTasks();
		}
	}


	private ObservableCollection<Ticket> _tasks
		= new ObservableCollection<Ticket>();

	/// <summary>
	/// The tickets for the current filter.
	/// </summary>
	public ObservableCollection<Ticket> Tasks
	{
		get => _tasks;
		private set
		{
			if (_tasks != value)
			{
				_tasks = value;
				OnPropertyChanged();
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;


	public TaskHomeViewModel(ITaskDAL taskDAL)
	{
		_taskDAL = taskDAL
			?? throw new ArgumentNullException(nameof(taskDAL));

		SelectedFilter = Filters[1]; 
	}

	/// <summary>
	/// Re-queries the DAL and updates Tasks.
	/// </summary>
	public void RefreshTasks()
	{
		var userId = UserSession.CurrentUserId;
		List<Ticket>? list = _selectedFilter == "Available"
			? _taskDAL!.GetAvailableTasksForUserGroups(userId)
			: _taskDAL!.GetTasksByAssignee(userId);

		if (list == null) list = new List<Ticket>();
		Tasks = new ObservableCollection<Ticket>(list);
	}

	protected void OnPropertyChanged([CallerMemberName] string? name = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
