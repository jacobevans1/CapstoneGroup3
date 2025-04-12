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

		var viewModel = new TicketViewModel
		{
			Project = project!,
			Board = board!
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
            SelectedUserId = ticket.AssignedTo!,
            CurrentUser = _singletonService.CurrentUser!,
            CurrentUserRole = _singletonService.CurrentUserRole,
            AssignedGroups = assignedGroups ?? new Dictionary<string, List<Group>>()
        };

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

		if (!ModelState.IsValid)
		{
			viewModel.Project = _projectRepository.GetProjectByIdAsync(viewModel.Project.Id!).Result!;
			viewModel.Board = _boardRepository.GetBoardByProjectIdAsync(viewModel.Project.Id!).Result!;
			return View(viewModel);
		}

		var ticket = _ticketRepository.Get(viewModel.Ticket.Id!);
		if (ticket == null)
		{
			TempData["ErrorMessage"] = "Ticket not found.";
			return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
		}

		ticket.Title = viewModel.Ticket.Title;
		ticket.Description = viewModel.Ticket.Description;
		ticket.AssignedTo = viewModel.SelectedUserId;
		ticket.Stage = viewModel.SelectedStageId;

		try
		{
			_ticketRepository.UpdateTicket(ticket);
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

		ticket.Stage = newStageId;

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
				}
			}
		}

		_ticketRepository.UpdateTicket(ticket);
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
