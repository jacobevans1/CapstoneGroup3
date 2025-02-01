using TicketAppDesktop.ViewModels;

namespace TicketAppDesktop
{
    public partial class CalculatorForm : Form
    {
        private readonly CalculatorViewModel _viewModel = new();

        public CalculatorForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _viewModel.Num1 = double.Parse(txtNum1.Text);
            _viewModel.Num2 = double.Parse(txtNum2.Text);
            _viewModel.Add();
            lblResult.Text = _viewModel.Result.ToString();
        }

        private void btnSubtract_Click(object sender, EventArgs e)
        {
            _viewModel.Num1 = double.Parse(txtNum1.Text);
            _viewModel.Num2 = double.Parse(txtNum2.Text);
            _viewModel.Subtract();
            lblResult.Text = _viewModel.Result.ToString();
        }

        private void btnMultiply_Click(object sender, EventArgs e)
        {
            _viewModel.Num1 = double.Parse(txtNum1.Text);
            _viewModel.Num2 = double.Parse(txtNum2.Text);
            _viewModel.Multiply();
            lblResult.Text = _viewModel.Result.ToString();
        }

        private void btnDivide_Click(object sender, EventArgs e)
        {
            _viewModel.Num1 = double.Parse(txtNum1.Text);
            _viewModel.Num2 = double.Parse(txtNum2.Text);
            _viewModel.Divide();
            lblResult.Text = _viewModel.Result.ToString();
        }
    }
}
