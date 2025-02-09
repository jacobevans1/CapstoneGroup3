
namespace TicketAppDesktop.Models
{
	public class Calculation
	{
		public int Id { get; set; }
		public double Value { get; set; }

		public Calculation(double value)
		{
			this.Value = value;
		}
		public Calculation(int id, double value)
		{
			this.Id = id;
			this.Value = value;
		}
	}
}
