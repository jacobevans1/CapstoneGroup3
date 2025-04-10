namespace TicketAppDesktop.Views
{
    partial class TicketAppHome
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Controls for adding a new group
        private Label lblGroupName;
        private TextBox txtGroupName;
        private Label lblDescription;
        private TextBox txtDescription;
        private Label lblManagerId;
        private TextBox txtManagerId;
        private Button btnAddGroup;

        // Control for displaying all groups
        private DataGridView dgvGroups;
        private Label lblGroups;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support — do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblGroupName = new Label();
            txtGroupName = new TextBox();
            lblDescription = new Label();
            txtDescription = new TextBox();
            lblManagerId = new Label();
            txtManagerId = new TextBox();
            btnAddGroup = new Button();
            dgvGroups = new DataGridView();
            lblGroups = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvGroups).BeginInit();
            SuspendLayout();
            // 
            // lblGroupName
            // 
            lblGroupName.AutoSize = true;
            lblGroupName.Location = new Point(23, 27);
            lblGroupName.Name = "lblGroupName";
            lblGroupName.Size = new Size(97, 20);
            lblGroupName.TabIndex = 0;
            lblGroupName.Text = "Group Name:";
            // 
            // txtGroupName
            // 
            txtGroupName.Location = new Point(23, 53);
            txtGroupName.Margin = new Padding(3, 4, 3, 4);
            txtGroupName.Name = "txtGroupName";
            txtGroupName.Size = new Size(228, 27);
            txtGroupName.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(23, 100);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(88, 20);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(23, 127);
            txtDescription.Margin = new Padding(3, 4, 3, 4);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(228, 27);
            txtDescription.TabIndex = 3;
            // 
            // lblManagerId
            // 
            lblManagerId.AutoSize = true;
            lblManagerId.Location = new Point(23, 173);
            lblManagerId.Name = "lblManagerId";
            lblManagerId.Size = new Size(90, 20);
            lblManagerId.TabIndex = 4;
            lblManagerId.Text = "Manager ID:";
            // 
            // txtManagerId
            // 
            txtManagerId.Location = new Point(23, 200);
            txtManagerId.Margin = new Padding(3, 4, 3, 4);
            txtManagerId.Name = "txtManagerId";
            txtManagerId.Size = new Size(228, 27);
            txtManagerId.TabIndex = 5;
            // 
            // btnAddGroup
            // 
            btnAddGroup.Location = new Point(23, 253);
            btnAddGroup.Margin = new Padding(3, 4, 3, 4);
            btnAddGroup.Name = "btnAddGroup";
            btnAddGroup.Size = new Size(229, 40);
            btnAddGroup.TabIndex = 6;
            btnAddGroup.Text = "Add Group";
            btnAddGroup.UseVisualStyleBackColor = true;
            btnAddGroup.Click += btnAddGroup_Click;
            // 
            // dgvGroups
            // 
            dgvGroups.AllowUserToAddRows = false;
            dgvGroups.AllowUserToDeleteRows = false;
            dgvGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGroups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGroups.Location = new Point(286, 53);
            dgvGroups.Margin = new Padding(3, 4, 3, 4);
            dgvGroups.MultiSelect = false;
            dgvGroups.Name = "dgvGroups";
            dgvGroups.ReadOnly = true;
            dgvGroups.RowHeadersWidth = 51;
            dgvGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGroups.Size = new Size(571, 400);
            dgvGroups.TabIndex = 7;
            // 
            // lblGroups
            // 
            lblGroups.AutoSize = true;
            lblGroups.Location = new Point(286, 27);
            lblGroups.Name = "lblGroups";
            lblGroups.Size = new Size(81, 20);
            lblGroups.TabIndex = 8;
            lblGroups.Text = "All Groups:";
            // 
            // TicketAppHome
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 533);
            Controls.Add(lblGroupName);
            Controls.Add(txtGroupName);
            Controls.Add(lblDescription);
            Controls.Add(txtDescription);
            Controls.Add(lblManagerId);
            Controls.Add(txtManagerId);
            Controls.Add(btnAddGroup);
            Controls.Add(lblGroups);
            Controls.Add(dgvGroups);
            Margin = new Padding(3, 4, 3, 4);
            Name = "TicketAppHome";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TicketApp - Manage Groups";
            ((System.ComponentModel.ISupportInitialize)dgvGroups).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
