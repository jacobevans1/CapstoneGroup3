using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TicketAppDesktop.DataLayer;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.ViewModels;

/// <summary>
/// Task home view model
/// </summary>
/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
public class TaskHomeViewModel : INotifyPropertyChanged
{

		
	/// <summary>
	/// Gets the filters. The two filter options
	/// </summary>
	/// <value>
	/// The filters.
	/// </value>
	public ObservableCollection<string> Filters { get; } =

		new ObservableCollection<string> { "Available", "My Tasks" };

	private string? _selectedFilter;

	/// <summary>
	/// Gets or sets the selected filter.
	/// </summary>
	/// <value>
	/// The selected filter.
	/// </value>
	public string? SelectedFilter
	{
		get => _selectedFilter;
		set
		{
			if (_selectedFilter != value)
			{
				_selectedFilter = value;
				OnPropertyChanged();
				RefreshTasks();
			}
		}
	}

	private ObservableCollection<Ticket> _tasks = new ObservableCollection<Ticket>();

	/// <summary>
	/// Gets the tasks.
	/// </summary>
	/// <value>
	/// The tasks.
	/// </value>
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

	/// <summary>
	/// Initializes a new instance of the <see cref="TaskHomeViewModel"/> class.
	/// </summary>
	public TaskHomeViewModel()
	{
		SelectedFilter = Filters[1];
	}

	/// <summary>
	/// Reloads the Tasks collection from the DAL based on the current filter.
	/// </summary>
	public void RefreshTasks()
	{
		var userId = UserSession.CurrentUserId;
		List<Ticket> list;

		if (SelectedFilter == "Available")
		{

			list = TicketDAL.GetAvailableTasksForUserGroups(userId);
		}

		else
		{

			list = TicketDAL.GetTasksByAssignee(userId);
		}

		Tasks = new ObservableCollection<Ticket>(list);
	}

	/// <summary>
	/// Occurs when a property value changes.
	/// </summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string name = null!) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

