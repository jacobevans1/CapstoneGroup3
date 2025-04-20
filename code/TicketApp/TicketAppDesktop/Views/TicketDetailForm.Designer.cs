namespace TicketAppDesktop.Views
{
	partial class TicketDetailForm
	{
		private System.ComponentModel.IContainer components = null;

		private Panel panelDetail;
		private Label lblDetailTitle;
		private TextBox txtDetailTitle;
		private Label lblDetailDescription;
		private TextBox txtDetailDescription;
		private Label lblDetailStage;
		private ComboBox cmbDetailStage;
		private CheckBox chkDetailAssigned;
		private Button btnDetailSave;
		private Button btnDetailCancel;
		private Label lblDetailHistory;
		private ListBox lstDetailHistory;

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
			panelDetail = new Panel();
			lblDetailTitle = new Label();
			txtDetailTitle = new TextBox();
			lblDetailDescription = new Label();
			txtDetailDescription = new TextBox();
			lblDetailStage = new Label();
			cmbDetailStage = new ComboBox();
			chkDetailAssigned = new CheckBox();
			btnDetailSave = new Button();
			btnDetailCancel = new Button();
			lblDetailHistory = new Label();
			lstDetailHistory = new ListBox();
			panelDetail.SuspendLayout();
			SuspendLayout();
			// 
			// panelDetail
			// 
			panelDetail.BackColor = SystemColors.Window;
			panelDetail.BorderStyle = BorderStyle.FixedSingle;
			panelDetail.Controls.Add(lblDetailTitle);
			panelDetail.Controls.Add(txtDetailTitle);
			panelDetail.Controls.Add(lblDetailDescription);
			panelDetail.Controls.Add(txtDetailDescription);
			panelDetail.Controls.Add(lblDetailStage);
			panelDetail.Controls.Add(cmbDetailStage);
			panelDetail.Controls.Add(chkDetailAssigned);
			panelDetail.Controls.Add(btnDetailSave);
			panelDetail.Controls.Add(btnDetailCancel);
			panelDetail.Controls.Add(lblDetailHistory);
			panelDetail.Controls.Add(lstDetailHistory);
			panelDetail.Location = new Point(10, 10);
			panelDetail.Margin = new Padding(2);
			panelDetail.Name = "panelDetail";
			panelDetail.Size = new Size(507, 680);
			panelDetail.TabIndex = 0;
			// 
			// lblDetailTitle
			// 
			lblDetailTitle.AutoSize = true;
			lblDetailTitle.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblDetailTitle.Location = new Point(16, 27);
			lblDetailTitle.Margin = new Padding(2, 0, 2, 0);
			lblDetailTitle.Name = "lblDetailTitle";
			lblDetailTitle.Size = new Size(50, 25);
			lblDetailTitle.TabIndex = 1;
			lblDetailTitle.Text = "Title";
			// 
			// txtDetailTitle
			// 
			txtDetailTitle.Location = new Point(16, 54);
			txtDetailTitle.Margin = new Padding(2);
			txtDetailTitle.Name = "txtDetailTitle";
			txtDetailTitle.Size = new Size(473, 27);
			txtDetailTitle.TabIndex = 2;
			// 
			// lblDetailDescription
			// 
			lblDetailDescription.AutoSize = true;
			lblDetailDescription.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblDetailDescription.Location = new Point(16, 100);
			lblDetailDescription.Margin = new Padding(2, 0, 2, 0);
			lblDetailDescription.Name = "lblDetailDescription";
			lblDetailDescription.Size = new Size(109, 25);
			lblDetailDescription.TabIndex = 3;
			lblDetailDescription.Text = "Description";
			// 
			// txtDetailDescription
			// 
			txtDetailDescription.Location = new Point(16, 127);
			txtDetailDescription.Margin = new Padding(2);
			txtDetailDescription.Multiline = true;
			txtDetailDescription.Name = "txtDetailDescription";
			txtDetailDescription.Size = new Size(473, 91);
			txtDetailDescription.TabIndex = 4;
			// 
			// lblDetailStage
			// 
			lblDetailStage.AutoSize = true;
			lblDetailStage.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblDetailStage.Location = new Point(16, 231);
			lblDetailStage.Margin = new Padding(2, 0, 2, 0);
			lblDetailStage.Name = "lblDetailStage";
			lblDetailStage.Size = new Size(60, 25);
			lblDetailStage.TabIndex = 5;
			lblDetailStage.Text = "Stage";
			// 
			// cmbDetailStage
			// 
			cmbDetailStage.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbDetailStage.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			cmbDetailStage.Location = new Point(16, 258);
			cmbDetailStage.Margin = new Padding(2);
			cmbDetailStage.Name = "cmbDetailStage";
			cmbDetailStage.Size = new Size(241, 33);
			cmbDetailStage.TabIndex = 6;
			// 
			// chkDetailAssigned
			// 
			chkDetailAssigned.AutoSize = true;
			chkDetailAssigned.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			chkDetailAssigned.Location = new Point(342, 262);
			chkDetailAssigned.Margin = new Padding(2);
			chkDetailAssigned.Name = "chkDetailAssigned";
			chkDetailAssigned.Size = new Size(111, 29);
			chkDetailAssigned.TabIndex = 7;
			chkDetailAssigned.Text = "Assigned";
			chkDetailAssigned.UseVisualStyleBackColor = true;
			// 
			// btnDetailSave
			// 
			btnDetailSave.BackColor = Color.IndianRed;
			btnDetailSave.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			btnDetailSave.ForeColor = Color.White;
			btnDetailSave.Location = new Point(104, 609);
			btnDetailSave.Margin = new Padding(2);
			btnDetailSave.Name = "btnDetailSave";
			btnDetailSave.Size = new Size(80, 40);
			btnDetailSave.TabIndex = 8;
			btnDetailSave.Text = "Save";
			btnDetailSave.UseVisualStyleBackColor = false;
			// 
			// btnDetailCancel
			// 
			btnDetailCancel.BackColor = Color.IndianRed;
			btnDetailCancel.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			btnDetailCancel.ForeColor = Color.White;
			btnDetailCancel.Location = new Point(282, 609);
			btnDetailCancel.Margin = new Padding(2);
			btnDetailCancel.Name = "btnDetailCancel";
			btnDetailCancel.Size = new Size(80, 40);
			btnDetailCancel.TabIndex = 9;
			btnDetailCancel.Text = "Cancel";
			btnDetailCancel.TextAlign = ContentAlignment.TopCenter;
			btnDetailCancel.UseVisualStyleBackColor = false;
			// 
			// lblDetailHistory
			// 
			lblDetailHistory.AutoSize = true;
			lblDetailHistory.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblDetailHistory.Location = new Point(16, 307);
			lblDetailHistory.Margin = new Padding(2, 0, 2, 0);
			lblDetailHistory.Name = "lblDetailHistory";
			lblDetailHistory.Size = new Size(75, 25);
			lblDetailHistory.TabIndex = 10;
			lblDetailHistory.Text = "History";
			// 
			// lstDetailHistory
			// 
			lstDetailHistory.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lstDetailHistory.ItemHeight = 25;
			lstDetailHistory.Location = new Point(16, 334);
			lstDetailHistory.Margin = new Padding(2);
			lstDetailHistory.Name = "lstDetailHistory";
			lstDetailHistory.Size = new Size(473, 229);
			lstDetailHistory.TabIndex = 11;
			// 
			// TicketDetailForm
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(531, 696);
			Controls.Add(panelDetail);
			Margin = new Padding(2);
			Name = "TicketDetailForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Ticket Details";
			panelDetail.ResumeLayout(false);
			panelDetail.PerformLayout();
			ResumeLayout(false);
		}
		#endregion
	}
}
