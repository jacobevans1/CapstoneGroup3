using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.ViewModels;

public interface ITicketDAL
{
	Ticket GetTicketById(string ticketId);
	void UpdateTicket(Ticket ticket);
}

public interface IStageDAL
{
	List<Stage> GetStagesForBoard(string boardId);
}

public interface ITicketHistoryDAL
{
	List<TicketHistory> GetHistoryByTicketId(string ticketId);
	void SaveHistoryEntry(TicketHistory entry);
}

public interface IUsersDAL
{
	string GetFullName(string userId);
}

public class TicketDetailViewModel : INotifyPropertyChanged
{
	private readonly ITicketDAL _ticketDAL;
	private readonly IStageDAL _stageDAL;
	private readonly ITicketHistoryDAL _historyDAL;
	private readonly IUsersDAL _usersDAL;

	public TicketDetailViewModel(
		ITicketDAL ticketDAL,
		IStageDAL stageDAL,
		ITicketHistoryDAL historyDAL,
		IUsersDAL usersDAL)
	{
		_ticketDAL = ticketDAL;
		_stageDAL = stageDAL;
		_historyDAL = historyDAL;
		_usersDAL = usersDAL;
	}

	private Ticket? _ticket;
	public Ticket? Ticket
	{
		get => _ticket;
		set
		{
			_ticket = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(Title));
			OnPropertyChanged(nameof(Description));
			OnPropertyChanged(nameof(SelectedStageId));
			OnPropertyChanged(nameof(Assigned));
		}
	}

	private ObservableCollection<Stage>? _stages;
	public ObservableCollection<Stage>? Stages
	{
		get => _stages;
		set { _stages = value; OnPropertyChanged(); }
	}

	private ObservableCollection<TicketHistory>? _history;
	public ObservableCollection<TicketHistory>? History
	{
		get => _history;
		set { _history = value; OnPropertyChanged(); }
	}

	public string Title
	{
		get => Ticket?.Title ?? "";
		set
		{
			if (Ticket != null)
			{
				Ticket.Title = value;
				OnPropertyChanged();
			}
		}
	}

	public string? Description
	{
		get => Ticket?.Description;
		set
		{
			if (Ticket != null)
			{
				Ticket.Description = value;
				OnPropertyChanged();
			}
		}
	}

	public string? SelectedStageId
	{
		get => Ticket?.Stage;
		set
		{
			if (Ticket != null)
			{
				Ticket.Stage = value;
				OnPropertyChanged();
			}
		}
	}

	public bool Assigned
	{
		get => !string.IsNullOrEmpty(Ticket?.AssignedTo);
		set
		{
			if (Ticket != null)
			{
				Ticket.AssignedTo = value
					? UserSession.CurrentUserId
					: null;
				OnPropertyChanged();
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string name = "")
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	public void Load(string ticketId)
	{
		var t = _ticketDAL.GetTicketById(ticketId)
				?? throw new InvalidOperationException("Ticket not found");
		Ticket = t;
		Stages = new ObservableCollection<Stage>(
			_stageDAL.GetStagesForBoard(t.BoardId!));
		History = new ObservableCollection<TicketHistory>(
			_historyDAL.GetHistoryByTicketId(ticketId));
	}

	public void SaveChanges()
	{
		var original = _ticketDAL.GetTicketById(Ticket!.Id!)
			?? throw new InvalidOperationException("Ticket not found");
		_ticketDAL.UpdateTicket(Ticket!);

		void LogIfChanged(string prop, string? oldVal, string? newVal)
		{
			string oldValue = oldVal ?? "";
			string newValue = newVal ?? "";

			switch (prop)
			{
				case "Stage":
					oldValue = _stageDAL.GetStagesForBoard("").Find(s => s.Id == oldVal!)?.Name ?? oldValue;
					newValue = _stageDAL.GetStagesForBoard("").Find(s => s.Id == newVal!)?.Name ?? newValue;
					break;
				case "AssignedTo":
					oldValue = string.IsNullOrEmpty(oldVal)
						? "Unassigned"
						: _usersDAL.GetFullName(oldVal);
					newValue = string.IsNullOrEmpty(newVal)
						? "Unassigned"
						: _usersDAL.GetFullName(newVal);
					break;
			}

			if (oldValue.Trim() == newValue.Trim())
				return;

			string desc = prop switch
			{
				"Stage" => $"{_usersDAL.GetFullName(UserSession.CurrentUserId)} moved stage from {oldValue} to {newValue}",
				"AssignedTo" => $"{_usersDAL.GetFullName(UserSession.CurrentUserId)} changed assignee from “{oldValue}” to “{newValue}”",
				_ => $"{_usersDAL.GetFullName(UserSession.CurrentUserId)} updated {prop} from “{oldValue}” to “{newValue}”"
			};

			var entry = new TicketHistory
			{
				Id = Guid.NewGuid().ToString(),
				TicketId = Ticket!.Id,
				PropertyChanged = prop,
				OldValue = oldValue,
				NewValue = newValue,
				ChangedByUserId = UserSession.CurrentUserId,
				ChangeDate = DateTime.Now,
				ChangeDescription = desc
			};

			_historyDAL.SaveHistoryEntry(entry);
			History?.Insert(0, entry);
		}

		LogIfChanged("Title", original.Title, Ticket!.Title);
		LogIfChanged("Description", original.Description, Ticket!.Description);
		LogIfChanged("AssignedTo", original.AssignedTo, Ticket!.AssignedTo);
		LogIfChanged("Stage", original.Stage, Ticket!.Stage);
	}
}
