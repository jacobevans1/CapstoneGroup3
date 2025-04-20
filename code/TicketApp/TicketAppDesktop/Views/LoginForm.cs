using TicketAppDesktop.ViewModels;

namespace TicketAppDesktop.Views;

public partial class LoginForm : Form
{
    private readonly LoginViewModel loginViewModel;

	/// <summary>
	/// Initializes a new instance of the <see cref="LoginForm"/> class.
	/// </summary>
	public LoginForm()
    {
        InitializeComponent();
        loginViewModel = new LoginViewModel();
    }

    private void loginBTN_Click(object sender, EventArgs e)
    {
        loginViewModel.User.UserName = usernameTB.Text;
        loginViewModel.InputPassword = passwordTB.Text;

        if (loginViewModel.Login())
        {
            var ticketAppHome = new TicketAppHome();
            ticketAppHome.Show();
            this.Hide();
        }
    }

    private void passwordTB_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            loginViewModel.User.UserName = usernameTB.Text;
            loginViewModel.InputPassword = passwordTB.Text;

            if (loginViewModel.Login())
            {                
                var ticketAppHome = new TicketAppHome();
                ticketAppHome.Show();
                this.Hide();               
            }

        }
    }
}
