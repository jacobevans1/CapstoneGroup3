
using Microsoft.AspNetCore.Mvc;
using TaskAppWeb.Controllers;
using TicketAppWeb.Models;

namespace TestTicketAppWeb
{
    public class TestHomeController
    {
        [Fact]
        public void Index_Get_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ViewData["FV"]);
        }

        [Fact]
        public void Index_Post_ValidModel_ReturnsViewWithFutureValue()
        {
            // Arrange
            var controller = new HomeController();
            var model = new FutureValueModel
            {
                MonthlyInvestment = 100,
                YearlyInterestRate = 5,
                Years = 10
            };

            // Act
            var result = controller.Index(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.CalculateFutureValue(), result.ViewData["FV"]);
        }

        [Fact]
        public void Index_Post_InvalidModel_ReturnsViewWithFVZero()
        {
            // Arrange
            var controller = new HomeController();
            var model = new FutureValueModel
            {
                MonthlyInvestment = 100,
                YearlyInterestRate = null, // Invalid because it's required
                Years = 10
            };

            controller.ModelState.AddModelError("YearlyInterestRate", "Required");

            // Act
            var result = controller.Index(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ViewData["FV"]);
        }
    }
}
