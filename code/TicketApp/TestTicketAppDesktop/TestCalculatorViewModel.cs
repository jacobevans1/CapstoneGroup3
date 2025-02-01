
using TicketAppDesktop.ViewModels;

namespace TestTicketAppDesktop
{
    public class TestCalculatorViewModel
    {
        private readonly CalculatorViewModel _viewModel = new();

        [Fact]
        public void Add_UpdatesResultCorrectly()
        {
            _viewModel.Num1 = 2;
            _viewModel.Num2 = 3;
            _viewModel.Add();
            Assert.Equal(5, _viewModel.Result);
        }

        [Fact]
        public void Subtract_UpdatesResultCorrectly()
        {
            _viewModel.Num1 = 5;
            _viewModel.Num2 = 2;
            _viewModel.Subtract();
            Assert.Equal(3, _viewModel.Result);
        }

        [Fact]
        public void Multiply_UpdatesResultCorrectly()
        {
            _viewModel.Num1 = 4;
            _viewModel.Num2 = 3;
            _viewModel.Multiply();
            Assert.Equal(12, _viewModel.Result);
        }

        [Fact]
        public void Divide_UpdatesResultCorrectly()
        {
            _viewModel.Num1 = 10;
            _viewModel.Num2 = 2;
            _viewModel.Divide();
            Assert.Equal(5, _viewModel.Result);
        }

        [Fact]
        public void Divide_ByZero_SetsResultToNaN()
        {
            _viewModel.Num1 = 10;
            _viewModel.Num2 = 0;
            _viewModel.Divide();
            Assert.Equal(double.NaN, _viewModel.Result);
        }
    }
}
