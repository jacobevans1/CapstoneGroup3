namespace TicketAppDesktop
{
	partial class CalculatorForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			txtNum1 = new TextBox();
			txtNum2 = new TextBox();
			lblResult = new Label();
			btnAdd = new Button();
			btnSubtract = new Button();
			btnDivide = new Button();
			btnMultiply = new Button();
			btnClear = new Button();
			dataGridViewNumbers = new DataGridView();
			label1 = new Label();
			((System.ComponentModel.ISupportInitialize)dataGridViewNumbers).BeginInit();
			SuspendLayout();
			// 
			// txtNum1
			// 
			txtNum1.BorderStyle = BorderStyle.None;
			txtNum1.Location = new Point(125, 87);
			txtNum1.Margin = new Padding(2);
			txtNum1.Name = "txtNum1";
			txtNum1.Size = new Size(105, 16);
			txtNum1.TabIndex = 0;
			// 
			// txtNum2
			// 
			txtNum2.BorderStyle = BorderStyle.None;
			txtNum2.Location = new Point(125, 128);
			txtNum2.Margin = new Padding(2);
			txtNum2.Name = "txtNum2";
			txtNum2.Size = new Size(105, 16);
			txtNum2.TabIndex = 1;
			// 
			// lblResult
			// 
			lblResult.AutoSize = true;
			lblResult.BackColor = SystemColors.Window;
			lblResult.Location = new Point(279, 170);
			lblResult.Margin = new Padding(2, 0, 2, 0);
			lblResult.Name = "lblResult";
			lblResult.Size = new Size(0, 15);
			lblResult.TabIndex = 2;
			// 
			// btnAdd
			// 
			btnAdd.FlatAppearance.BorderSize = 0;
			btnAdd.FlatStyle = FlatStyle.System;
			btnAdd.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
			btnAdd.Location = new Point(279, 88);
			btnAdd.Margin = new Padding(2);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new Size(78, 20);
			btnAdd.TabIndex = 3;
			btnAdd.Text = "+";
			btnAdd.UseVisualStyleBackColor = true;
			btnAdd.Click += btnAdd_Click;
			// 
			// btnSubtract
			// 
			btnSubtract.FlatAppearance.BorderSize = 0;
			btnSubtract.FlatStyle = FlatStyle.System;
			btnSubtract.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
			btnSubtract.Location = new Point(279, 124);
			btnSubtract.Margin = new Padding(2);
			btnSubtract.Name = "btnSubtract";
			btnSubtract.Size = new Size(78, 20);
			btnSubtract.TabIndex = 4;
			btnSubtract.Text = "-";
			btnSubtract.UseVisualStyleBackColor = true;
			btnSubtract.Click += btnSubtract_Click;
			// 
			// btnDivide
			// 
			btnDivide.FlatAppearance.BorderSize = 0;
			btnDivide.FlatStyle = FlatStyle.System;
			btnDivide.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
			btnDivide.Location = new Point(382, 122);
			btnDivide.Margin = new Padding(2);
			btnDivide.Name = "btnDivide";
			btnDivide.Size = new Size(78, 20);
			btnDivide.TabIndex = 5;
			btnDivide.Text = ":";
			btnDivide.UseVisualStyleBackColor = true;
			btnDivide.Click += btnDivide_Click;
			// 
			// btnMultiply
			// 
			btnMultiply.FlatAppearance.BorderSize = 0;
			btnMultiply.FlatStyle = FlatStyle.System;
			btnMultiply.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
			btnMultiply.Location = new Point(382, 88);
			btnMultiply.Margin = new Padding(2);
			btnMultiply.Name = "btnMultiply";
			btnMultiply.Size = new Size(78, 20);
			btnMultiply.TabIndex = 6;
			btnMultiply.Text = "x";
			btnMultiply.UseVisualStyleBackColor = true;
			btnMultiply.Click += btnMultiply_Click;
			// 
			// btnClear
			// 
			btnClear.Location = new Point(125, 170);
			btnClear.Name = "btnClear";
			btnClear.Size = new Size(75, 23);
			btnClear.TabIndex = 7;
			btnClear.Text = "Clear";
			btnClear.UseVisualStyleBackColor = true;
			btnClear.Click += btnClear_Click;
			// 
			// dataGridViewNumbers
			// 
			dataGridViewNumbers.AllowUserToAddRows = false;
			dataGridViewNumbers.AllowUserToDeleteRows = false;
			dataGridViewNumbers.AllowUserToResizeColumns = false;
			dataGridViewNumbers.AllowUserToResizeRows = false;
			dataGridViewNumbers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewNumbers.BackgroundColor = SystemColors.ActiveCaption;
			dataGridViewNumbers.BorderStyle = BorderStyle.None;
			dataGridViewNumbers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewNumbers.Location = new Point(498, 32);
			dataGridViewNumbers.MultiSelect = false;
			dataGridViewNumbers.Name = "dataGridViewNumbers";
			dataGridViewNumbers.ReadOnly = true;
			dataGridViewNumbers.RowHeadersVisible = false;
			dataGridViewNumbers.ScrollBars = ScrollBars.Vertical;
			dataGridViewNumbers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dataGridViewNumbers.Size = new Size(240, 202);
			dataGridViewNumbers.TabIndex = 8;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(498, 9);
			label1.Name = "label1";
			label1.Size = new Size(90, 15);
			label1.TabIndex = 9;
			label1.Text = "Recently Added";
			// 
			// CalculatorForm
			// 
			AccessibleRole = AccessibleRole.Clock;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.ActiveCaption;
			ClientSize = new Size(773, 270);
			Controls.Add(label1);
			Controls.Add(dataGridViewNumbers);
			Controls.Add(btnClear);
			Controls.Add(btnMultiply);
			Controls.Add(btnDivide);
			Controls.Add(btnSubtract);
			Controls.Add(btnAdd);
			Controls.Add(lblResult);
			Controls.Add(txtNum2);
			Controls.Add(txtNum1);
			Margin = new Padding(2);
			Name = "CalculatorForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "CalculatorForm";
			((System.ComponentModel.ISupportInitialize)dataGridViewNumbers).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private TextBox txtNum1;
        private TextBox txtNum2;
        private Label lblResult;
        private Button btnAdd;
        private Button btnSubtract;
        private Button btnDivide;
        private Button btnMultiply;
		private Button btnClear;
		private DataGridView dataGridViewNumbers;
		private Label label1;
	}
}
