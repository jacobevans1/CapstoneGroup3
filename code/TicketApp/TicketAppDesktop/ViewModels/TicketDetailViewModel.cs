using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TicketAppDesktop.DataLayer;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.ViewModels;

/// <summary>
/// Ticket details view model
/// </summary>
/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
public class TicketDetailViewModel : INotifyPropertyChanged
{
	private Ticket? _ticket;
	private ObservableCollection<Stage>? _stages;
	private ObservableCollection<TicketHistory>? _history;

	/// <summary>
	/// Occurs when a property value changes.
	/// </summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// Gets or sets the ticket.
	/// </summary>
	/// <value>
	/// The ticket.
	/// </value>
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

	/// <summary>
	/// Gets or sets the stages.
	/// </summary>
	/// <value>
	/// The stages.
	/// </value>
	public ObservableCollection<Stage>? Stages
	{
		get => _stages;
		set { _stages = value; OnPropertyChanged(); }
	}

	/// <summary>
	/// Gets or sets the history.
	/// </summary>
	/// <value>
	/// The history.
	/// </value>
	public ObservableCollection<TicketHistory>? History
	{
		get => _history;
		set { _history = value; OnPropertyChanged(); }
	}

	/// <summary>
	/// Gets or sets the title.
	/// </summary>
	/// <value>
	/// The title.
	/// </value>
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

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	/// <value>
	/// The description.
	/// </value>
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

	/// <summary>
	/// Gets or sets the selected stage identifier.
	/// </summary>
	/// <value>
	/// The selected stage identifier.
	/// </value>
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

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="TicketDetailViewModel"/> is assigned.
	/// </summary>
	/// <value>
	///   <c>true</c> if assigned; otherwise, <c>false</c>.
	/// </value>
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

	/// <summary>
	/// Loads the specified ticket identifier.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	/// <exception cref="System.InvalidOperationException">Ticket not found</exception>
	public void Load(string ticketId)
	{
		Ticket = TicketDAL.GetTicketById(ticketId)
			?? throw new InvalidOperationException("Ticket not found");
		Stages = new ObservableCollection<Stage>(
			StageDAL.GetStagesForBoard(Ticket.BoardId!));
		History = new ObservableCollection<TicketHistory>(
			TicketHistoryDAL.GetHistoryByTicketId(ticketId));
	}

	/// <summary>
	/// Saves the changes.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">Ticket not found</exception>
	public void SaveChanges()
	{
		var original = TicketDAL.GetTicketById(Ticket!.Id!)
			?? throw new InvalidOperationException("Ticket not found");

		TicketDAL.UpdateTicket(Ticket!);

		var updaterId = UserSession.CurrentUserId;
		var updaterName = UsersDAL.GetFullName(updaterId);

		void LogIfChanged(string prop, string? oldId, string? newId)
		{
			string oldVal = oldId ?? "", newVal = newId ?? "";

			switch (prop)
			{
				case "Stage":
					oldVal = StageDAL.GetStageById(oldId!)?.Name ?? oldVal;
					newVal = StageDAL.GetStageById(newId!)?.Name ?? newVal;
					break;
				case "AssignedTo":
					oldVal = string.IsNullOrEmpty(oldId)
						? "Unassigned"
						: UsersDAL.GetFullName(oldId);
					newVal = string.IsNullOrEmpty(newId)
						? "Unassigned"
						: UsersDAL.GetFullName(newId);
					break;
			}

			if ((oldVal ?? "").Trim() == (newVal ?? "").Trim())
				return;

			string desc = prop switch
			{
				"Stage" => $"{updaterName} moved stage from {oldVal} to {newVal}",
				"AssignedTo" => $"{updaterName} changed assignee from “{oldVal}” to “{newVal}”",
				_ => $"{updaterName} updated {prop} from “{oldVal}” to “{newVal}”"
			};

			var entry = new TicketHistory
			{
				Id = Guid.NewGuid().ToString(),
				TicketId = Ticket!.Id!,
				PropertyChanged = prop,
				OldValue = oldVal,
				NewValue = newVal,
				ChangedByUserId = updaterId,
				ChangeDate = DateTime.Now,
				ChangeDescription = desc
			};

			TicketHistoryDAL.SaveHistoryEntry(entry);
			History?.Insert(0, entry);
		}

		LogIfChanged("Title", original.Title, Ticket!.Title);
		LogIfChanged("Description", original.Description, Ticket!.Description);
		LogIfChanged("AssignedTo", original.AssignedTo, Ticket!.AssignedTo);
		LogIfChanged("Stage", original.Stage, Ticket!.Stage);
	}

	protected void OnPropertyChanged([CallerMemberName] string name = "")
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
