using Microsoft.AspNetCore.Mvc;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;
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
			var newStageName = viewModel.NewStageName;
			var groupId = viewModel.SelectedGroupId;

			try
			{
				_boardRepository.AddStage(boardId, newStageName, groupId);
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
			var stageId = viewModel.SelectedStageId;
			var newStatusName = viewModel.NewStageName;

			try
			{
				_boardRepository.RenameStage(stageId, newStatusName);
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
			var stageId = viewModel.SelectedStageId;
			var groupId = viewModel.SelectedGroupId;

			try
			{
				_boardRepository.AssignGroupToStage(boardId, stageId, groupId);
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
			var stageId = viewModel.SelectedStageId;

			try
			{
				_boardRepository.DeleteStage(boardId, stageId);
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
		}


		/// <summary>
		/// Moves a stage left or right on the board.
		/// </summary>
		/// <param name="viewModel"></param>
		[HttpPost]
		[HttpPost]
		public IActionResult MoveStage(BoardViewModel viewModel)
		{
			var boardId = viewModel.Board.Id;
			var stageId = viewModel.SelectedStageId;
			var direction = viewModel.SelectedDirection;

			try
			{
				var boardStages = _boardRepository.GetBoardStages(boardId).OrderBy(bs => bs.StageOrder).ToList();
				var stageIndex = boardStages.FindIndex(bs => bs.StageId == stageId);

				if (stageIndex == -1)
					return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });

				if (direction == "left" && stageIndex > 0)
				{
					SwapStageOrder(boardStages, stageIndex, stageIndex - 1);
				}
				else if (direction == "right" && stageIndex < boardStages.Count - 1)
				{
					SwapStageOrder(boardStages, stageIndex, stageIndex + 1);
				}

				_boardRepository.SaveBoardStages(boardStages);

				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Board", new { projectId = viewModel.Project.Id });
			}
		}


		private void SwapStageOrder(List<BoardStage> boardStages, int index1, int index2)
		{
			int tempOrder = boardStages[index1].StageOrder;
			boardStages[index1].StageOrder = boardStages[index2].StageOrder;
			boardStages[index2].StageOrder = tempOrder;
		}


		private void LoadIndexViewData(BoardViewModel vm, string projectId)
		{
			var board = _boardRepository.GetBoardByProjectIdAsync(projectId).Result;
			var stages = _boardRepository.GetStages(board.Id);
			var assignedGroups = _boardRepository.GetAllAssignedGroupsForStages(board.Id);

			vm.Board = board;
			vm.Project = board.Project;
			vm.Stages = stages;
			vm.AssignedGroups = assignedGroups;
		}
	}
}
