using TicketAppDesktop.ViewModels;

namespace TicketAppDesktop.Views
{
	public partial class CalculatorForm : Form
	{
		private readonly CalculatorViewModel _viewModel = new();

		public CalculatorForm()
		{
			InitializeComponent();
			LoadNumbersIntoGrid();
			dataGridViewNumbers.Columns[0].Visible = false;
		}

		private void LoadNumbersIntoGrid()
		{
			var numbers = _viewModel.GetNumbers().OrderByDescending(n => n.Id).ToList();
			dataGridViewNumbers.DataSource = numbers;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			_viewModel.Num1 = double.Parse(txtNum1.Text);
			_viewModel.Num2 = double.Parse(txtNum2.Text);
			_viewModel.Add();
			_viewModel.SendNumber();
			lblResult.Text = _viewModel.Result.ToString();
			LoadNumbersIntoGrid();
		}

		private void btnSubtract_Click(object sender, EventArgs e)
		{
			_viewModel.Num1 = double.Parse(txtNum1.Text);
			_viewModel.Num2 = double.Parse(txtNum2.Text);
			_viewModel.Subtract();
			_viewModel.SendNumber();
			lblResult.Text = _viewModel.Result.ToString();
			LoadNumbersIntoGrid();
		}

		private void btnMultiply_Click(object sender, EventArgs e)
		{
			_viewModel.Num1 = double.Parse(txtNum1.Text);
			_viewModel.Num2 = double.Parse(txtNum2.Text);
			_viewModel.Multiply();
			_viewModel.SendNumber();
			lblResult.Text = _viewModel.Result.ToString();
			LoadNumbersIntoGrid();
		}

		private void btnDivide_Click(object sender, EventArgs e)
		{
			_viewModel.Num1 = double.Parse(txtNum1.Text);
			_viewModel.Num2 = double.Parse(txtNum2.Text);
			_viewModel.Divide();
			_viewModel.SendNumber();
			lblResult.Text = _viewModel.Result.ToString();
			LoadNumbersIntoGrid();
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			txtNum1.Text = "";
			txtNum2.Text = "";
			lblResult.Text = "";
			LoadNumbersIntoGrid();
		}
	}
}
