using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Controllers
{
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
}
