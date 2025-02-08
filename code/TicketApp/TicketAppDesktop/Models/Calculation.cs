
namespace TicketAppDesktop.Models
{
	public class Calculation
	{
		public string id { get; set; }
		public double result { get; set; }

		public Calculation(double result)
		{
			this.result = result;
		}
	}
}
