
namespace TicketAppDesktop.Models
{
    public class CalculatorModel
    {
        public double Add(double a, double b) => a + b;
        public double Subtract(double a, double b) => a - b;
        public double Multiply(double a, double b) => a * b;
        public double Divide(double a, double b)
        {
            if (b == 0) throw new DivideByZeroException("Cannot divide by zero.");

            var result = a / b;

            var calculation = new Calculation(result);
            //send to db

			return result;
        }
    }
}
