using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TicketAppDesktop.DataLayer;
using TicketAppDesktop.Models;


namespace TicketAppDesktop.ViewModels;

/// <summary>
/// Ticket Data layer interface to make the view model testable
/// </summary>
public interface ITicketDAL
{
	/// <summary>
	/// Gets the ticket by identifier.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	/// <returns></returns>
	Ticket GetTicketById(string ticketId);

	/// <summary>
	/// Updates the ticket.
	/// </summary>
	/// <param name="ticket">The ticket.</param>
	void UpdateTicket(Ticket ticket);

}

/// <summary>
/// Stage Data layer interface to make the view model testable
/// </summary>
public interface IStageDAL
{
	/// <summary>
	/// Gets the stages for board.
	/// </summary>
	/// <param name="boardId">The board identifier.</param>
	/// <returns></returns>
	List<Stage> GetStagesForBoard(string boardId);
}

/// <summary>
/// Ticket History Data layer interface to make the view model testable
/// </summary>
public interface ITicketHistoryDAL
{
	/// <summary>
	/// Gets the history by ticket identifier.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	/// <returns></returns>
	List<TicketHistory> GetHistoryByTicketId(string ticketId);

	/// <summary>
	/// Saves the history entry.
	/// </summary>
	/// <param name="entry">The entry.</param>
	void SaveHistoryEntry(TicketHistory entry);
}

/// <summary>
/// User Data layer interface to make the view model testable
/// </summary>
public interface IUsersDAL
{
	/// <summary>
	/// Gets the full name.
	/// </summary>
	/// <param name="userId">The user identifier.</param>
	/// <returns></returns>
	string GetFullName(string userId);
}

/// <summary>
/// Ticket details view model
/// </summary>
/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
public class TicketDetailViewModel : INotifyPropertyChanged
{

	private readonly ITicketDAL _ticketDAL;
	private readonly IStageDAL _stageDAL;
	private readonly ITicketHistoryDAL _historyDAL;
	private readonly IUsersDAL _usersDAL;



	/// <summary>
	/// Initializes a new instance of the <see cref="TicketDetailViewModel"/> class.
	/// </summary>
	/// <param name="ticketDAL">The ticket dal.</param>
	/// <param name="stageDAL">The stage dal.</param>
	/// <param name="historyDAL">The history dal.</param>
	/// <param name="usersDAL">The users dal.</param>
	/// <exception cref="System.ArgumentNullException">
	/// ticketDAL
	/// or
	/// stageDAL
	/// or
	/// historyDAL
	/// or
	/// usersDAL
	/// </exception>
	public TicketDetailViewModel(
		ITicketDAL ticketDAL,
		IStageDAL stageDAL,
		ITicketHistoryDAL historyDAL,
		IUsersDAL usersDAL)
	{

		_ticketDAL = ticketDAL ?? throw new ArgumentNullException(nameof(ticketDAL));

		_stageDAL = stageDAL ?? throw new ArgumentNullException(nameof(stageDAL));

		_historyDAL = historyDAL ?? throw new ArgumentNullException(nameof(historyDAL));

		_usersDAL = usersDAL ?? throw new ArgumentNullException(nameof(usersDAL));

	}

	private Ticket? _ticket;

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

	private ObservableCollection<Stage>? _stages;

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

	private ObservableCollection<TicketHistory>? _history;

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

		get => Ticket!.Description;
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
	/// Occurs when a property value changes.
	/// </summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	// safe-call so no NRE if no subscribers

	/// <summary>
	/// Called when [property changed].
	/// </summary>
	/// <param name="name">The name.</param>
	protected void OnPropertyChanged([CallerMemberName] string name = "")

		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	/// <summary>
	/// Loads the specified ticket identifier.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	/// <exception cref="System.InvalidOperationException">Ticket not found</exception>
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

	/// <summary>
	/// Saves the changes.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">Ticket not found</exception>
	public void SaveChanges()
	{

		var original = _ticketDAL.GetTicketById(Ticket!.Id!)

			?? throw new InvalidOperationException("Ticket not found");

		if (!string.IsNullOrEmpty(Ticket.AssignedTo) &&
		!IsUserValidForStage(Ticket.AssignedTo, Ticket.BoardId!, Ticket.Stage!))
		{
			Ticket.AssignedTo = null;
		}

		_ticketDAL.UpdateTicket(Ticket!);

		void LogIfChanged(string prop, string? oldVal, string? newVal)
		{

			string oldValue = oldVal ?? "";

			string newValue = newVal ?? "";



			switch (prop)
			{

				case "Stage":

					oldValue = _stageDAL.GetStagesForBoard("")

								.Find(s => s.Id == oldVal!)?.Name

							 ?? oldValue;

					newValue = _stageDAL.GetStagesForBoard("")

								.Find(s => s.Id == newVal!)?.Name

							 ?? newValue;

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

			History!.Insert(0, entry);

		}

		LogIfChanged("Title", original.Title, Ticket!.Title);

		LogIfChanged("Description", original.Description, Ticket!.Description);

		LogIfChanged("AssignedTo", original.AssignedTo, Ticket!.AssignedTo);

		LogIfChanged("Stage", original.Stage, Ticket!.Stage);

	}

	private bool IsUserValidForStage(string userId, string boardId, string stageId)
	{
		if (string.IsNullOrWhiteSpace(userId) || userId.ToLower() == "Unassigned")
			return false;

		using var conn = new SqlConnection(Connection.ConnectionString);
		conn.Open();

		const string query = @"
		SELECT COUNT(*) 
		FROM BoardStageGroups bsg
		JOIN GroupUser gu ON bsg.GroupId = gu.GroupId
		WHERE bsg.BoardId = @BoardId AND bsg.StageId = @StageId AND gu.MemberId = @UserId";

		using var cmd = new SqlCommand(query, conn);
		cmd.Parameters.AddWithValue("@BoardId", boardId);
		cmd.Parameters.AddWithValue("@StageId", stageId);
		cmd.Parameters.AddWithValue("@UserId", userId);

		int count = Convert.ToInt32(cmd.ExecuteScalar());
		return count > 0;
	}

	public bool IsUserValidForSelectedStage()
	{
		if (Ticket == null || string.IsNullOrEmpty(Ticket.Stage))
			return false;

		return IsUserValidForStage(UserSession.CurrentUserId, Ticket.BoardId!, Ticket.Stage);
	}
}

