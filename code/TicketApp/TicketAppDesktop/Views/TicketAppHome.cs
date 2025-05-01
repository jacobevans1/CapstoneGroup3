using System.ComponentModel;
using TicketAppDesktop.DataLayer;
using TicketAppDesktop.Models;
using TicketAppDesktop.ViewModels;

namespace TicketAppDesktop.Views;

/// <summary>
/// The home page
/// </summary>
public partial class TicketAppHome : Form
{
	private readonly TaskHomeViewModel _viewModel;

	public TicketAppHome()
	{
		InitializeComponent();

		// inject our static-DAL adapter
		_viewModel = new TaskHomeViewModel(new TaskDalAdapter());

		cmbFilter.DataSource = _viewModel.Filters;
		cmbFilter.SelectedIndexChanged += CmbFilter_SelectedIndexChanged;
		_viewModel.PropertyChanged += ViewModel_PropertyChanged;
		_viewModel.RefreshTasks();
	}

	private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(_viewModel.Tasks))
			RefreshTaskButtons();
	}

	private void CmbFilter_SelectedIndexChanged(object? sender, EventArgs e)
	{
		_viewModel.SelectedFilter = cmbFilter.SelectedItem as string ?? string.Empty;
		RefreshTaskButtons();
	}

	private void RefreshTaskButtons()
	{
		flpTasks.Controls.Clear();

		foreach (var ticket in _viewModel.Tasks)
		{
			var btn = new Button
			{
				Text = ticket.Title,
				Tag = ticket.Id,
				Width = flpTasks.ClientSize.Width - SystemInformation.VerticalScrollBarWidth,
				Height = 40,
				TextAlign = ContentAlignment.MiddleLeft,
				Margin = new Padding(3),
				BackColor = Color.IndianRed,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 11, FontStyle.Bold),
			};
			btn.Click += TaskButton_Click;
			flpTasks.Controls.Add(btn);
		}
	}

	private void TaskButton_Click(object? sender, EventArgs e)
	{
		if (sender is Button btn && btn.Tag is string ticketId)
		{
			using var detailForm = new TicketDetailForm(ticketId);
			if (detailForm.ShowDialog() == DialogResult.OK)
				_viewModel.RefreshTasks();
		}
	}

	/// <summary>
	/// Adapter from our ITaskDAL to the static TicketDAL.
	/// </summary>
	private class TaskDalAdapter : ITaskDAL
	{
		public List<Ticket> GetAvailableTasksForUserGroups(string userId)
			=> TicketDAL.GetAvailableTasksForUserGroups(userId);

		public List<Ticket> GetTasksByAssignee(string userId)
			=> TicketDAL.GetTasksByAssignee(userId);
	}

	private void logoutButton_Click(object sender, EventArgs e)
	{
		// Reset session state
		UserSession.Reset();

		// Show login form
		var loginForm = new LoginForm();
		loginForm.Show();

		// Close the current form
		this.Close();
	}

}
