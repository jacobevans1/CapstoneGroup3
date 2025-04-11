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
		/// Displays the ticket creation form for a specific board.
		/// </summary>
		/// <param name="projectId"></param>
		[HttpGet]
		public IActionResult AddTicket(string projectId)
		{
			var project = _projectRepository.GetProjectByIdAsync(projectId).Result;
			var board = _boardRepository.GetBoardByProjectIdAsync(projectId).Result;

			var viewModel = new TicketViewModel
			{
				Project = project,
				Board = board,
				CurrentUser = _singletonService.CurrentUser,
				CurrentUserRole = _singletonService.CurrentUserRole
			};

			viewModel.Project.Id = projectId;

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
			var newTicket = CreateTicketObject(viewModel);

			try
			{
				_ticketRepository.AddTicket(newTicket);
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Sorry, ticket creation failed.";
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}

			TempData["SuccessMessage"] = $"{newTicket.Title} added successfully.";
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
				AssignedTo = viewModel.Ticket.AssignedTo,
				IsComplete = false,
				BoardId = viewModel.Board.Id
			};
			return ticket;
		}
	}
}
