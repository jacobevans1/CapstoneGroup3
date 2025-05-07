using TicketAppDesktop.DataLayer;
using TicketAppDesktop.Models;
using TicketAppDesktop.ViewModels;

namespace TicketAppDesktop.Views;

/// <summary>
/// Ticket details view
/// </summary>
public partial class TicketDetailForm : Form
{
	private readonly TicketDetailViewModel _viewModel;

	/// <summary>
	/// Initializes a new instance of the <see cref="TicketDetailForm"/> class.
	/// </summary>
	/// <param name="ticketId">The ticket identifier.</param>
	public TicketDetailForm(string ticketId)
	{
		InitializeComponent();

		_viewModel = new TicketDetailViewModel(
			new TicketDalAdapter(),
			new StageDalAdapter(),
			new TicketHistoryDalAdapter(),
			new UsersDalAdapter()
		);

		_viewModel.Load(ticketId);

		txtDetailTitle.DataBindings.Add(
			"Text",
			_viewModel,
			nameof(_viewModel.Title),
			false,
			DataSourceUpdateMode.OnPropertyChanged);

		txtDetailDescription.DataBindings.Add(
			"Text",
			_viewModel,
			nameof(_viewModel.Description),
			false,
			DataSourceUpdateMode.OnPropertyChanged);

		cmbDetailStage.DataSource = _viewModel.Stages;
		cmbDetailStage.DisplayMember = nameof(Stage.Name);
		cmbDetailStage.ValueMember = nameof(Stage.Id);
		cmbDetailStage.DataBindings.Add(
			"SelectedValue",
			_viewModel,
			nameof(_viewModel.SelectedStageId),
			true,
			DataSourceUpdateMode.OnPropertyChanged);

		chkDetailAssigned.DataBindings.Add(
			"Checked",
			_viewModel,
			nameof(_viewModel.Assigned),
			true,
			DataSourceUpdateMode.OnPropertyChanged);

		lstDetailHistory.DataSource = _viewModel.History;
		lstDetailHistory.DisplayMember = nameof(TicketHistory.ChangeDescription);

		btnDetailSave.Click += BtnDetailSave_Click!;
		btnDetailCancel.Click += (s, e) => Close();
	}

	private void BtnDetailSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (!_viewModel.IsUserValidForSelectedStage())
			{
				var result = MessageBox.Show(
					"You are not part of a group assigned to the selected stage. Once moved, you may not be able to recover this ticket. Do you want to proceed?",
					"Warning",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning);

				if (result == DialogResult.No)
				{
					return;
				}
			}

			_viewModel.SaveChanges();

			MessageBox.Show(
				"Ticket updated successfully.",
				"Success",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);

			lstDetailHistory.DataSource = null;
			lstDetailHistory.DataSource = _viewModel.History;
			lstDetailHistory.DisplayMember = nameof(TicketHistory.ChangeDescription);

			DialogResult = DialogResult.OK;
			Close();
		}
		catch (Exception ex)
		{
			MessageBox.Show(
				"Error saving ticket: " + ex.Message,
				"Error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
	}

	private class TicketDalAdapter : ITicketDAL
	{
		public Ticket GetTicketById(string ticketId)
			=> TicketDAL.GetTicketById(ticketId);

		public void UpdateTicket(Ticket ticket)
			=> TicketDAL.UpdateTicket(ticket);
	}

	private class StageDalAdapter : IStageDAL
	{
		public List<Stage> GetStagesForBoard(string boardId)
			=> StageDAL.GetStagesForBoard(boardId);
	}

	private class TicketHistoryDalAdapter : ITicketHistoryDAL
	{
		public List<TicketHistory> GetHistoryByTicketId(string ticketId)
			=> TicketHistoryDAL.GetHistoryByTicketId(ticketId);

		public void SaveHistoryEntry(TicketHistory entry)
			=> TicketHistoryDAL.SaveHistoryEntry(entry);
	}

	private class UsersDalAdapter : IUsersDAL
	{
		public string GetFullName(string userId)
			=> UsersDAL.GetFullName(userId);
	}
}