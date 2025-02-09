using System.ComponentModel.DataAnnotations;

namespace TicketAppWeb.Models
{
	public class Number
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Please enter a number to send.")]
		public double Value { get; set; }

		public Number(double value)
		{
			Value = value;
		}
	}
}
