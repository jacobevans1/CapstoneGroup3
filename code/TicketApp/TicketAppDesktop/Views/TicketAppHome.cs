using TicketAppDesktop.ViewModels;

namespace TicketAppDesktop.Views
{
    public partial class TicketAppHome : Form
    {
        private readonly GroupViewModel _viewModel = new();

        public TicketAppHome(string loggedInUser)
        {
            InitializeComponent();
            LoadGroupsIntoGrid();
            // Optionally hide certain columns like the generated Id.
            if (dgvGroups.Columns["Id"] != null)
            {
                dgvGroups.Columns["Id"].Visible = false;
            }
        }

        // Loads groups into the DataGridView, ordering them by creation time.
        private void LoadGroupsIntoGrid()
        {
            // Order groups by descending CreatedAt date (or by Id if desired).
            var groups = _viewModel.Groups
                                   .OrderByDescending(g => g.CreatedAt)
                                   .ToList();
            dgvGroups.DataSource = groups;
        }

        // New group entry button event.
        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            // Assign input values to the view model.
            _viewModel.GroupName = txtGroupName.Text;
            _viewModel.Description = txtDescription.Text;
            _viewModel.ManagerId = txtManagerId.Text;

            // Add the new group through the view model.
            _viewModel.AddGroup();

            // Clear the input fields after adding the group.
            txtGroupName.Clear();
            txtDescription.Clear();
            txtManagerId.Clear();

            // Refresh the grid to update the list of groups.
            LoadGroupsIntoGrid();
        }
    }
}
