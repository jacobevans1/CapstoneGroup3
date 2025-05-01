namespace TicketAppDesktop.Views
{
	partial class TicketAppHome
	{
		private System.ComponentModel.IContainer components = null;

		private Panel panelMain;
		private Label lblFilter;
		private ComboBox cmbFilter;
		private FlowLayoutPanel flpTasks;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			panelMain = new Panel();
			logoutButton = new Button();
			lblFilter = new Label();
			cmbFilter = new ComboBox();
			flpTasks = new FlowLayoutPanel();
			panelMain.SuspendLayout();
			SuspendLayout();
			// 
			// panelMain
			// 
			panelMain.BackColor = Color.White;
			panelMain.BorderStyle = BorderStyle.Fixed3D;
			panelMain.Controls.Add(logoutButton);
			panelMain.Controls.Add(lblFilter);
			panelMain.Controls.Add(cmbFilter);
			panelMain.Controls.Add(flpTasks);
			panelMain.Location = new Point(9, 8);
			panelMain.Margin = new Padding(2);
			panelMain.Name = "panelMain";
			panelMain.Size = new Size(444, 512);
			panelMain.TabIndex = 0;
			// 
			// logoutButton
			// 
			logoutButton.BackColor = Color.IndianRed;
			logoutButton.FlatStyle = FlatStyle.Flat;
			logoutButton.Font = new Font("Showcard Gothic", 9F);
			logoutButton.ForeColor = Color.White;
			logoutButton.Location = new Point(355, 11);
			logoutButton.Name = "logoutButton";
			logoutButton.Size = new Size(75, 31);
			logoutButton.TabIndex = 4;
			logoutButton.Text = "Logout";
			logoutButton.UseVisualStyleBackColor = false;
			logoutButton.Click += logoutButton_Click;
			// 
			// lblFilter
			// 
			lblFilter.AutoSize = true;
			lblFilter.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblFilter.Location = new Point(97, 14);
			lblFilter.Margin = new Padding(2, 0, 2, 0);
			lblFilter.Name = "lblFilter";
			lblFilter.Size = new Size(47, 20);
			lblFilter.TabIndex = 1;
			lblFilter.Text = "View:";
			// 
			// cmbFilter
			// 
			cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbFilter.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			cmbFilter.Items.AddRange(new object[] { "Available", "My Tasks" });
			cmbFilter.Location = new Point(151, 11);
			cmbFilter.Margin = new Padding(2);
			cmbFilter.Name = "cmbFilter";
			cmbFilter.Size = new Size(141, 27);
			cmbFilter.TabIndex = 2;
			cmbFilter.SelectedIndexChanged += CmbFilter_SelectedIndexChanged;
			// 
			// flpTasks
			// 
			flpTasks.AutoScroll = true;
			flpTasks.FlowDirection = FlowDirection.TopDown;
			flpTasks.Location = new Point(14, 47);
			flpTasks.Margin = new Padding(2);
			flpTasks.Name = "flpTasks";
			flpTasks.Size = new Size(416, 451);
			flpTasks.TabIndex = 3;
			flpTasks.WrapContents = false;
			// 
			// TicketAppHome
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(465, 522);
			Controls.Add(panelMain);
			Margin = new Padding(2);
			Name = "TicketAppHome";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "TicketApp - Tasks Overview";
			panelMain.ResumeLayout(false);
			panelMain.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private Button logoutButton;
	}
}
