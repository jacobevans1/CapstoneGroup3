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
			panelMain.Controls.Add(lblFilter);
			panelMain.Controls.Add(cmbFilter);
			panelMain.Controls.Add(flpTasks);
			panelMain.Location = new Point(10, 10);
			panelMain.Margin = new Padding(2, 2, 2, 2);
			panelMain.Name = "panelMain";
			panelMain.Size = new Size(507, 681);
			panelMain.TabIndex = 0;
			// 
			// lblFilter
			// 
			lblFilter.AutoSize = true;
			lblFilter.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblFilter.Location = new Point(111, 18);
			lblFilter.Margin = new Padding(2, 0, 2, 0);
			lblFilter.Name = "lblFilter";
			lblFilter.Size = new Size(58, 25);
			lblFilter.TabIndex = 1;
			lblFilter.Text = "View:";
			// 
			// cmbFilter
			// 
			cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbFilter.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			cmbFilter.Items.AddRange(new object[] { "Available", "My Tasks" });
			cmbFilter.Location = new Point(173, 15);
			cmbFilter.Margin = new Padding(2, 2, 2, 2);
			cmbFilter.Name = "cmbFilter";
			cmbFilter.Size = new Size(161, 33);
			cmbFilter.TabIndex = 2;
			cmbFilter.SelectedIndexChanged += cmbFilter_SelectedIndexChanged;
			// 
			// flpTasks
			// 
			flpTasks.AutoScroll = true;
			flpTasks.FlowDirection = FlowDirection.TopDown;
			flpTasks.Location = new Point(16, 63);
			flpTasks.Margin = new Padding(2, 2, 2, 2);
			flpTasks.Name = "flpTasks";
			flpTasks.Size = new Size(476, 601);
			flpTasks.TabIndex = 3;
			flpTasks.WrapContents = false;
			// 
			// TicketAppHome
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(531, 696);
			Controls.Add(panelMain);
			Margin = new Padding(2, 2, 2, 2);
			Name = "TicketAppHome";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "TicketApp - Tasks Overview";
			panelMain.ResumeLayout(false);
			panelMain.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
	}
}
