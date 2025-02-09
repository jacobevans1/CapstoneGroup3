
using TicketAppWeb.Models;

namespace TestTicketAppWeb
{
    public class TestFutureValueModel
    {
        [Fact]
        public void CalculateFutureValue_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            var model = new FutureValueModel
            {
                MonthlyInvestment = 100,
                YearlyInterestRate = 5,
                Years = 10
            };

            // Act
            var result = model.CalculateFutureValue();

            // Assert
            Assert.NotNull(result);
            Assert.True(result > 0);
        }

        [Fact]
        public void CalculateFutureValue_NullValues_ReturnsNull()
        {
            // Arrange
            var model = new FutureValueModel
            {
                MonthlyInvestment = 100,
                YearlyInterestRate = null,
                Years = null
            };

            // Act
            var result = model.CalculateFutureValue();

            // Assert
            Assert.Equal(0, result);
        }
    }
}
