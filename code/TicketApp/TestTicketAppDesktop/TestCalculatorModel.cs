using TicketAppDesktop.Models;

namespace TestTicketAppDesktop
{
    public class TestCalculatorModel
    {
        private readonly CalculatorModel _calculator = new();

        [Fact]
        public void Add_ReturnsCorrectSum()
        {
            Assert.Equal(5, _calculator.Add(2, 3));
        }

        [Fact]
        public void Subtract_ReturnsCorrectDifference()
        {
            Assert.Equal(1, _calculator.Subtract(3, 2));
        }

        [Fact]
        public void Multiply_ReturnsCorrectProduct()
        {
            Assert.Equal(6, _calculator.Multiply(2, 3));
        }

        [Fact]
        public void Divide_ReturnsCorrectQuotient()
        {
            Assert.Equal(2, _calculator.Divide(6, 3));
        }

        [Fact]
        public void Divide_ByZero_ThrowsException()
        {
            Assert.Throws<DivideByZeroException>(() => _calculator.Divide(5, 0));
        }
    }
}