// Capstone Group 3
// Spring 2025
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

namespace TicketAppWeb.Models.DataLayer.Repositories
{
	/// <summary>
	/// The BoardRepository class implements IBoardRepository
	/// </summary>
	public class BoardRepository : Repository<Board>, IBoardRepository
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoardRepository"/> class.
		/// </summary>
		/// <param name="ctx"></param>
		public BoardRepository(TicketAppContext ctx) : base(ctx)
		{
		}


	}
}
