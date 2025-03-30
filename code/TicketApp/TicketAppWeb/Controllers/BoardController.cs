using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.ViewModels;

// Capstone Group 3
// Spring 2025
namespace TicketAppWeb.Controllers
{
	/// <summary>
	/// The BoardController class represents a controller for board-related actions.
	/// </summary>
	public class BoardController : Controller
	{
		private readonly SingletonService _singletonService;
		private readonly IProjectRepository _projectRepository;
		private readonly IBoardRepository _boardRepository;


		/// <summary>
		/// Initializes a new instance of the UserController class.
		/// </summary>
		public BoardController(SingletonService singletonService, IProjectRepository projectRepository,
			IBoardRepository boardRepository)
		{
			_singletonService = singletonService;
			_projectRepository = projectRepository;
			_boardRepository = boardRepository;
		}


		/// <summary>
		/// Displays the board index view.
		/// </summary>
		public IActionResult Index(string projectId)
		{
			var viewModel = new BoardViewModel();

			viewModel.CurrentUser = _singletonService.CurrentUser;
			viewModel.CurrentUserRole = _singletonService.CurrentUserRole;

			LoadIndexViewData(viewModel, projectId);

			return View(viewModel);
		}


		/// <summary>
		/// Adds a new column to the board.
		/// </summary>
		/// <param name="viewModel"></param>
		[HttpPost]
		public IActionResult AddStage(BoardViewModel viewModel)
		{
			var boardId = viewModel.Board.Id;
			var newStatusName = viewModel.NewStageName;
			var groupId = viewModel.SelectedGroupId;

			try
			{
				_boardRepository.AddStatus(boardId, newStatusName, groupId);
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });

			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
		}


		/// <summary>
		/// Renames a stage on the board.
		/// </summary>
		/// <param name="viewModel"></param>
		[HttpPost]
		public IActionResult RenameStage(BoardViewModel viewModel)
		{
			var statusId = viewModel.SelectedStageId;
			var newStatusName = viewModel.NewStageName;

			try
			{
				_boardRepository.RenameStage(statusId, newStatusName);
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
		}


		/// <summary>
		/// Assigns a group to a stage.
		/// </summary>
		/// <param name="viewModel"></param>
		[HttpPost]
		public IActionResult AssignGroupToStage(BoardViewModel viewModel)
		{
			var boardId = viewModel.Board.Id;
			var statusId = viewModel.SelectedStageId;
			var groupId = viewModel.SelectedGroupId;

			try
			{
				_boardRepository.AssignGroupToStage(boardId, statusId, groupId);
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
		}


		/// <summary>
		/// Deletes a stage from the board.
		/// </summary>
		/// <param name="viewModel"></param>
		public IActionResult DeleteStage(BoardViewModel viewModel)
		{
			var boardId = viewModel.Board.Id;
			var statusId = viewModel.SelectedStageId;

			try
			{
				_boardRepository.DeleteStage(boardId, statusId);
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
		}


		private void LoadIndexViewData(BoardViewModel vm, string projectId)
		{
			var board = _boardRepository.GetBoardByProjectIdAsync(projectId).Result;
			var statuses = _boardRepository.GetBoardStages(board.Id);
			var assignedGroups = _boardRepository.GetAllAssignedGroupsForStages(board.Id);

			vm.Board = board;
			vm.Project = board.Project;
			vm.Stages = statuses;
			vm.AssignedGroups = assignedGroups;
		}
	}
}
