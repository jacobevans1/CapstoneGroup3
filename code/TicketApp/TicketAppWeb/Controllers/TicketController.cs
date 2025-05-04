using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Controllers;

/// <summary>
/// The TicketController class represents a controller for ticket-related actions.
/// </summary>
public class TicketController : Controller
{
	private readonly SingletonService _singletonService;
	private readonly IProjectRepository _projectRepository;
	private readonly IBoardRepository _boardRepository;
	private readonly ITicketRepository _ticketRepository;
	private readonly IUserRepository _userRepository;


	/// <summary>
	/// Initializes a new instance of the UserController class.
	/// </summary>
	public TicketController(SingletonService singletonService, IProjectRepository projectRepository, IBoardRepository boardRepository, ITicketRepository ticketRepository, IUserRepository userRepository)
	{
		_singletonService = singletonService;
		_projectRepository = projectRepository;
		_boardRepository = boardRepository;
		_ticketRepository = ticketRepository;
		_userRepository = userRepository;
	}


	/// <summary>
	/// Displays the form to add a new ticket.
	/// </summary>
	/// <param name="projectId"></param>
	/// <param name="stageId"></param>
	[HttpGet]
	public IActionResult AddTicket(string projectId, string stageId)
	{
		var project = _projectRepository.GetProjectByIdAsync(projectId).Result;
		var board = _boardRepository.GetBoardByProjectIdAsync(projectId).Result;
		var assignedGroups = _boardRepository.GetBoardStageGroups(board!.Id);

		var viewModel = new TicketViewModel
		{
			Project = project!,
			Board = board!,
			AssignedGroups = assignedGroups
		};

		viewModel.Project!.Id = projectId;
		viewModel.SelectedStageId = stageId;

		viewModel.CurrentUser = _singletonService.CurrentUser!;
		viewModel.CurrentUserRole = _singletonService.CurrentUserRole;

		foreach (var group in viewModel.Project.Groups)
		{
			var users = _userRepository.GetUsersByGroupId(group.Id);
			group.Members = users.ToList();
		}

		foreach (var group in viewModel.Project.Groups)
		{
			if (group.ManagerId == _singletonService.CurrentUser!.Id)
			{
				foreach (var member in group.Members)
				{
					if (!viewModel.EligibleAssignees.Any(u => u.Id == member.Id))
					{
						viewModel.EligibleAssignees.Add(member);
					}
				}
			}

			else if (group.Members.Any(m => m.Id == _singletonService.CurrentUser.Id))
			{
				if (!viewModel.EligibleAssignees.Any(u => u.Id == _singletonService.CurrentUser.Id))
				{
					viewModel.EligibleAssignees.Add(_singletonService.CurrentUser);
				}
			}

			else if (_singletonService.CurrentUserRole == "Admin")
			{
				foreach (var member in group.Members)
				{
					viewModel.EligibleAssignees.Add(member);
				}
			}
		}

		return View(viewModel);
	}


	/// <summary>
	/// Adds a new ticket to the board.
	/// </summary>
	/// <param name="viewModel"></param>
	[HttpPost]
	public IActionResult AddTicket(TicketViewModel viewModel)
	{
		viewModel.CurrentUser = _singletonService.CurrentUser!;
		viewModel.CurrentUserRole = _singletonService.CurrentUserRole;

		if (viewModel.SelectedUserId == "Unassigned")
		{
			viewModel.Ticket.AssignedTo = null;
		}

		var newTicket = CreateTicketObject(viewModel);

		try
		{
			_ticketRepository.AddTicket(newTicket);
		}
		catch (Exception)
		{
			TempData["ErrorMessage"] = $"Sorry, ticket creation failed.";
			return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
		}

		TempData["SuccessMessage"] = $"{newTicket.Title} added successfully.";
		return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
	}


	/// <summary>
	/// Edits the ticket.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	/// <param name="projectId">The project identifier.</param>
	/// <param name="boardId">The board identifier.</param>
	/// <param name="stageId">The stage identifier.</param>
	/// <returns></returns>
	[HttpGet]
	public IActionResult EditTicket(string ticketId, string projectId, string boardId, string stageId)
	{
		var ticket = _ticketRepository.Get(ticketId);
		if (ticket == null)
		{
			TempData["ErrorMessage"] = "Ticket not found.";
			return RedirectToAction("Index", "Board", new { projectId });
		}

		var project = _projectRepository.GetProjectByIdAsync(projectId).Result;
		var board = _boardRepository.GetBoardByProjectIdAsync(projectId).Result;

		if (project != null && project.Groups != null)
		{
			foreach (var group in project.Groups)
			{
				if (group.Members == null || !group.Members.Any())
				{
					group.Members = _userRepository.GetUsersByGroupId(group.Id).ToList();
				}
			}
		}

		var assignedGroups = _boardRepository.GetBoardStageGroups(board!.Id);

		var viewModel = new TicketViewModel
		{
			Ticket = ticket,
			Project = project!,
			Board = board,
			SelectedStageId = stageId,
			CurrentUser = _singletonService.CurrentUser!,
			CurrentUserRole = _singletonService.CurrentUserRole,
			AssignedGroups = assignedGroups ?? new Dictionary<string, List<Group>>()
		};

		if (ticket.AssignedTo == null)
		{
			viewModel.SelectedUserId = "Unassigned";
		}

		if (assignedGroups != null && assignedGroups.ContainsKey(stageId))
		{
			foreach (var group in assignedGroups[stageId])
			{
				if (group.ManagerId == _singletonService.CurrentUser!.Id)
				{
					foreach (var member in group.Members)
					{
						if (!viewModel.EligibleAssignees.Any(u => u.Id == member.Id))
						{
							viewModel.EligibleAssignees.Add(member);
						}
					}
				}

				else if (group.Members.Any(m => m.Id == _singletonService.CurrentUser.Id))
				{
					if (!viewModel.EligibleAssignees.Any(u => u.Id == _singletonService.CurrentUser.Id))
					{
						viewModel.EligibleAssignees.Add(_singletonService.CurrentUser);
					}
				}

				else if (_singletonService.CurrentUserRole == "Admin")
				{
					foreach (var member in group.Members)
					{
						viewModel.EligibleAssignees.Add(member);
					}
				}
			}
		}

		ticket.AssignedToUser = _userRepository.Get(ticket.AssignedTo!)!;

		return View(viewModel);
	}


	/// <summary>
	/// Edits the ticket.
	/// </summary>
	/// <param name="viewModel">The view model.</param>
	/// <returns></returns>
	[HttpPost]
	public IActionResult EditTicket(TicketViewModel viewModel)
	{
		viewModel.CurrentUser = _singletonService.CurrentUser!;
		viewModel.CurrentUserRole = _singletonService.CurrentUserRole;

		if (viewModel.SelectedUserId == "Unassigned")
		{
			viewModel.Ticket.AssignedTo = null;
		}

		if (!ModelState.IsValid)
		{
			viewModel.Project = _projectRepository.GetProjectByIdAsync(viewModel.Project.Id!).Result!;
			viewModel.Board = _boardRepository.GetBoardByProjectIdAsync(viewModel.Project.Id!).Result!;
			return View(viewModel);
		}

		var existingTicket = _ticketRepository.Get(viewModel.Ticket.Id!);
		if (existingTicket == null)
		{
			TempData["ErrorMessage"] = "Ticket not found.";
			return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
		}

		var updatedTicket = new Ticket
		{
			Id = existingTicket.Id,
			BoardId = existingTicket.BoardId,
			Title = viewModel.Ticket.Title,
			Description = viewModel.Ticket.Description,
			AssignedTo = viewModel.SelectedUserId,
			Stage = viewModel.SelectedStageId,
			CreatedBy = existingTicket.CreatedBy,
			CreatedDate = existingTicket.CreatedDate
		};

		try
		{
			var updater = _singletonService.CurrentUser!;
			var updaterName = $"{updater.FirstName} {updater.LastName}";

			LogChangeIfNeeded("Title", existingTicket.Title, updatedTicket.Title, existingTicket, updater.Id, updaterName);
			LogChangeIfNeeded("AssignedTo", existingTicket.AssignedTo, updatedTicket.AssignedTo, existingTicket, updater.Id, updaterName);
			LogChangeIfNeeded("Description", existingTicket.Description, updatedTicket.Description, existingTicket, updater.Id, updaterName);
			LogChangeIfNeeded("Stage", existingTicket.Stage, updatedTicket.Stage, existingTicket, updater.Id, updaterName);



			_ticketRepository.UpdateTicket(existingTicket, updatedTicket);
			TempData["SuccessMessage"] = "Ticket updated successfully.";
		}
		catch (Exception)
		{
			TempData["ErrorMessage"] = "Ticket update failed.";
			viewModel.Project = _projectRepository.GetProjectByIdAsync(viewModel.Project.Id!).Result!;
			viewModel.Board = _boardRepository.GetBoardByProjectIdAsync(viewModel.Project.Id!).Result!;
			return View(viewModel);
		}


		return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
	}



	/// <summary>
	/// Moves the ticket.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	/// <param name="newStageId">The new stage identifier.</param>
	/// <param name="boardId">The board identifier.</param>
	/// <param name="projectId">The project identifier.</param>
	/// <returns></returns>
	[HttpGet]
	public IActionResult MoveTicket(string ticketId, string newStageId, string boardId, string projectId)
	{
		var ticket = _ticketRepository.Get(ticketId);
		if (ticket == null)
		{
			TempData["ErrorMessage"] = "Ticket not found.";
			return RedirectToAction("Index", "Board", new { projectId });
		}

		var originalStage = ticket.Stage;
		var originalAssignedTo = ticket.AssignedTo;

		ticket.Stage = newStageId;

		var updater = _singletonService.CurrentUser!;
		var updaterName = $"{updater.FirstName} {updater.LastName}";

		if (!string.IsNullOrEmpty(ticket.AssignedTo))
		{
			var boardStageGroups = _boardRepository.GetBoardStageGroups(boardId);
			if (boardStageGroups != null && boardStageGroups.ContainsKey(newStageId))
			{
				var targetGroups = boardStageGroups[newStageId];
				bool userInGroup = false;

				var project = _projectRepository.GetProjectByIdAsync(projectId).Result;
				if (project != null && project.Groups != null)
				{
					var relevantGroups = project.Groups.Where(g => targetGroups.Any(tg => tg.Id == g.Id));

					foreach (var group in relevantGroups)
					{
						var members = group.Members;
						if (members == null || !members.Any())
						{
							members = _userRepository.GetUsersByGroupId(group.Id).ToList();
						}

						if (members.Any(m => m.Id == ticket.AssignedTo))
						{
							userInGroup = true;
							break;
						}
					}
				}

				if (!userInGroup)
				{
					ticket.AssignedTo = null;

					LogChangeIfNeeded("AssignedTo", originalAssignedTo, null, ticket, updater.Id, updaterName);
				}
			}
		}

		LogChangeIfNeeded("Stage", originalStage, newStageId, ticket, updater.Id, updaterName);

		var updatedTicket = new Ticket
		{
			Id = ticket.Id,
			BoardId = ticket.BoardId,
			Title = ticket.Title,
			Description = ticket.Description,
			AssignedTo = ticket.AssignedTo,
			Stage = ticket.Stage,
			CreatedBy = ticket.CreatedBy,
			CreatedDate = ticket.CreatedDate
		};

		_ticketRepository.UpdateTicket(ticket, updatedTicket);

		TempData["SuccessMessage"] = "Ticket moved successfully.";
		return RedirectToAction("Index", "Board", new { projectId });
	}




	/// <summary>
	/// Deletes a ticket from the board.
	/// </summary>
	/// <param name="viewModel"></param>
	public IActionResult DeleteTicket(BoardViewModel viewModel)
	{
		try
		{
			_ticketRepository.DeleteTicket(viewModel.SelectedTicketId);
		}
		catch (Exception)
		{
			TempData["ErrorMessage"] = $"Sorry, deleting ticket failed.";
			return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
		}

		TempData["SuccessMessage"] = $"Deleted ticket successfully.";
		return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
	}


	/// <summary>
	/// Displays the details of a ticket.
	/// </summary>
	/// <param name="ticketId"></param>
	/// <param name="projectId"></param>
	[HttpGet]
	public IActionResult Details(string ticketId, string projectId)
	{
		var ticket = _ticketRepository.GetTicketWithHistory(ticketId);
		if (ticket == null)
		{
			return NotFound();
		}

		var project = _projectRepository.GetProjectByIdAsync(projectId).Result;
		var board = _boardRepository.GetBoardByProjectIdAsync(projectId).Result;

		var stageName = _boardRepository.GetStages(board.Id).FirstOrDefault(s => s.Id == ticket.Stage)?.Name ?? "Unknown";

		var viewModel = new TicketDetailsViewModel
		{
			Ticket = ticket,
			Project = project,
			Board = board,
			StageName = stageName,
			History = ticket.History.OrderByDescending(h => h.ChangeDate).ToList(),
			CurrentUser = _singletonService.CurrentUser,
			CurrentUserRole = _singletonService.CurrentUserRole
		};

		return View(viewModel);
	}

	private void LogChangeIfNeeded(string propertyName, string? oldVal, string? newVal, Ticket existingTicket, string updaterId, string updaterName)
	{
		if (oldVal?.Trim() != newVal?.Trim())
		{
			string timestamp = DateTime.Now.ToString("g");
			string description = $"{updaterName} updated the {propertyName} to \"{newVal}\" at {timestamp}";

			if (propertyName == "AssignedTo")
			{
				var assigneeName = _userRepository.Get(newVal!)?.FullName ?? "Unassigned";
				description = $"{updaterName} updated the Assignee to \"{assigneeName}\" at {timestamp}";
			}

			if (propertyName == "Stage")
			{
				var oldStageName = _boardRepository.GetStages(existingTicket.BoardId)
					.FirstOrDefault(s => s.Id == oldVal)?.Name ?? "Unknown";

				var newStageName = _boardRepository.GetStages(existingTicket.BoardId)
					.FirstOrDefault(s => s.Id == newVal)?.Name ?? "Unknown";

				description = $"{updaterName} moved the ticket from {oldStageName} to {newStageName} at {timestamp}";
			}

			existingTicket.History.Add(new TicketHistory
			{
				Id = Guid.NewGuid().ToString(),
				TicketId = existingTicket.Id,
				PropertyChanged = propertyName,
				OldValue = oldVal,
				NewValue = newVal,
				ChangedByUserId = updaterId,
				ChangeDate = DateTime.Now,
				ChangeDescription = description
			});
		}
	}


	private Ticket CreateTicketObject(TicketViewModel viewModel)
	{
		var ticket = new Ticket
		{
			Id = Guid.NewGuid().ToString(),
			Title = viewModel.Ticket.Title,
			Description = viewModel.Ticket.Description,
			CreatedDate = DateTime.Now,
			CreatedBy = viewModel.CurrentUser.Id,
			AssignedTo = viewModel.SelectedUserId,
			Stage = viewModel.SelectedStageId,
			IsComplete = false,
			BoardId = viewModel.Board.Id
		};
		return ticket;
	}
}
