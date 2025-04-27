using Moq;
using TicketAppDesktop.Models;
using TicketAppDesktop.Services;
using TicketAppDesktop.ViewModels;

namespace TestTicketAppDesktop.ViewModels;

public class LoginViewModelTests
{
	private readonly Mock<IAuthenticationService> _auth;
	private readonly Mock<IMessageBoxService> _msg;
	private readonly LoginViewModel _vm;

	public LoginViewModelTests()
	{
		_auth = new Mock<IAuthenticationService>();
		_msg = new Mock<IMessageBoxService>();
		_vm = new LoginViewModel(_auth.Object, _msg.Object);
	}

	[Fact]
	public void Ctor_Throws_WhenAuthServiceIsNull()
	{
		Assert.Throws<ArgumentNullException>(
			() => new LoginViewModel(null!, _msg.Object));
	}

	[Fact]
	public void Ctor_Throws_WhenMessageBoxServiceIsNull()
	{
		Assert.Throws<ArgumentNullException>(
			() => new LoginViewModel(_auth.Object, null!));
	}

	[Fact]
	public void Login_Fails_WhenUsernameEmpty()
	{
		_vm.User.UserName = "";
		_vm.InputPassword = "whatever";

		var ok = _vm.Login();
		Assert.False(ok);

		_msg.Verify(m => m.ShowError("Please enter your username."), Times.Once);
		_auth.VerifyNoOtherCalls();
	}

	[Fact]
	public void Login_Fails_WhenPasswordEmpty()
	{
		_vm.User.UserName = "joe";
		_vm.InputPassword = "";

		var ok = _vm.Login();
		Assert.False(ok);

		_msg.Verify(m => m.ShowError("Please enter your password."), Times.Once);
		_auth.VerifyNoOtherCalls();
	}

	[Fact]
	public void Login_Fails_WhenUserNotFound()
	{
		_vm.User.UserName = "joe";
		_vm.InputPassword = "pw";
		_auth.Setup(a => a.GetUserDetails("joe")).Returns((UserDetails?)null);

		var ok = _vm.Login();
		Assert.False(ok);

		_auth.Verify(a => a.GetUserDetails("joe"), Times.Once);
		_msg.Verify(m => m.ShowError("Invalid username or password. Please try again."), Times.Once);
	}

	[Fact]
	public void Login_Fails_WhenPasswordWrong()
	{
		var ud = new UserDetails { UserName = "joe", PasswordHash = "hash" };
		_vm.User.UserName = "joe";
		_vm.InputPassword = "pw";
		_auth.Setup(a => a.GetUserDetails("joe")).Returns(ud);
		_auth.Setup(a => a.VerifyPassword("pw", "hash")).Returns(false);

		var ok = _vm.Login();
		Assert.False(ok);

		_auth.Verify(a => a.VerifyPassword("pw", "hash"), Times.Once);
		_msg.Verify(m => m.ShowError("Invalid username or password. Please try again."), Times.Once);
	}

	[Fact]
	public void Login_Succeeds_AndSetsAllFlags()
	{
		var ud = new UserDetails
		{
			Id = "u1",
			UserName = "joe",
			FirstName = "Joe",
			LastName = "Smith",
			PasswordHash = "h",
			Role = "ADMIN"
		};

		_vm.User.UserName = "joe";
		_vm.InputPassword = "pw";
		_auth.Setup(a => a.GetUserDetails("joe")).Returns(ud);
		_auth.Setup(a => a.VerifyPassword("pw", "h")).Returns(true);
		_auth.Setup(a => a.IsProjectLeader("u1")).Returns(true);
		_auth.Setup(a => a.IsGroupManager("u1")).Returns(false);

		var ok = _vm.Login();
		Assert.True(ok);

		Assert.Equal("u1", UserSession.CurrentUserId);
		Assert.Contains("joe (Joe Smith)", _vm.LoggedInUser);
		Assert.Equal("ADMIN", _vm.UserRole);
		Assert.True(_vm.IsProjectLeader);
		Assert.False(_vm.IsGroupManager);

		// on success, no error popup
		_msg.VerifyNoOtherCalls();
	}
}
