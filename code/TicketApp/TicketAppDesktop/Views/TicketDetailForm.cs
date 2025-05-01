using TicketAppDesktop.ViewModels;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.Views;

/// <summary>
/// Ticket details view
/// </summary>
/// <seealso cref="System.Windows.Forms.Form" />
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

		_viewModel = new TicketDetailViewModel();
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
}
