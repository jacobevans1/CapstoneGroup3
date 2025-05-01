using TicketAppDesktop.Services;
using TicketAppDesktop.ViewModels;

namespace TicketAppDesktop.Views
{
	public partial class LoginForm : Form
	{
		private readonly LoginViewModel _loginViewModel;

		public LoginForm()
		{
			InitializeComponent();

			// inject real ADO-NET auth + real MsgBox
			var auth = new LoginDAL();
			var msgBox = new MessageBoxService();
			_loginViewModel = new LoginViewModel(auth, msgBox);
		}

		private void loginBTN_Click(object sender, EventArgs e)
		{
			_loginViewModel.User.UserName = usernameTB.Text;
			_loginViewModel.InputPassword = passwordTB.Text;

			if (_loginViewModel.Login())
			{
				var home = new TicketAppHome();
				home.Show();
				Hide();
			}
		}

		private void passwordTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;

				_loginViewModel.User.UserName = usernameTB.Text;
				_loginViewModel.InputPassword = passwordTB.Text;

				if (_loginViewModel.Login())
				{
					var home = new TicketAppHome();
					home.Show();
					Hide();
				}
			}
		}
	}
}
