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
            SuspendLayout();
            // 
            // txtNum1
            // 
            txtNum1.BorderStyle = BorderStyle.None;
            txtNum1.Location = new Point(179, 145);
            txtNum1.Name = "txtNum1";
            txtNum1.Size = new Size(150, 24);
            txtNum1.TabIndex = 0;
            // 
            // txtNum2
            // 
            txtNum2.BorderStyle = BorderStyle.None;
            txtNum2.Location = new Point(179, 214);
            txtNum2.Name = "txtNum2";
            txtNum2.Size = new Size(150, 24);
            txtNum2.TabIndex = 1;
            // 
            // lblResult
            // 
            lblResult.AutoSize = true;
            lblResult.BackColor = SystemColors.Window;
            lblResult.Location = new Point(398, 283);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(0, 25);
            lblResult.TabIndex = 2;
            // 
            // btnAdd
            // 
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.System;
            btnAdd.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAdd.Location = new Point(398, 146);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(112, 34);
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
            btnSubtract.Location = new Point(398, 207);
            btnSubtract.Name = "btnSubtract";
            btnSubtract.Size = new Size(112, 34);
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
            btnDivide.Location = new Point(546, 204);
            btnDivide.Name = "btnDivide";
            btnDivide.Size = new Size(112, 34);
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
            btnMultiply.Location = new Point(546, 146);
            btnMultiply.Name = "btnMultiply";
            btnMultiply.Size = new Size(112, 34);
            btnMultiply.TabIndex = 6;
            btnMultiply.Text = "x";
            btnMultiply.UseVisualStyleBackColor = true;
            btnMultiply.Click += btnMultiply_Click;
            // 
            // CalculatorForm
            // 
            AccessibleRole = AccessibleRole.Clock;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(800, 450);
            Controls.Add(btnMultiply);
            Controls.Add(btnDivide);
            Controls.Add(btnSubtract);
            Controls.Add(btnAdd);
            Controls.Add(lblResult);
            Controls.Add(txtNum2);
            Controls.Add(txtNum1);
            Name = "CalculatorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CalculatorForm";
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
    }
}
