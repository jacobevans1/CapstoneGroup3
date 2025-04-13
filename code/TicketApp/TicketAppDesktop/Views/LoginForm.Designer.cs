namespace TicketAppDesktop.Views
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox2 = new PictureBox();
            panel1 = new Panel();
            pictureBox3 = new PictureBox();
            panel2 = new Panel();
            loginBTN = new Button();
            usernameTB = new TextBox();
            passwordTB = new TextBox();
            panel3 = new Panel();
            panel4 = new Panel();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Image = Properties.Resources.red_user_icon;
            pictureBox2.Location = new Point(61, 194);
            pictureBox2.Margin = new Padding(2, 3, 2, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(117, 43);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.IndianRed;
            panel1.ForeColor = SystemColors.ButtonShadow;
            panel1.Location = new Point(61, 243);
            panel1.Margin = new Padding(2, 3, 2, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(520, 1);
            panel1.TabIndex = 5;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.Image = Properties.Resources.security_icon;
            pictureBox3.Location = new Point(61, 303);
            pictureBox3.Margin = new Padding(2, 3, 2, 3);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(117, 43);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 1;
            pictureBox3.TabStop = false;
            // 
            // panel2
            // 
            panel2.BackColor = Color.IndianRed;
            panel2.ForeColor = SystemColors.ButtonShadow;
            panel2.Location = new Point(61, 352);
            panel2.Margin = new Padding(2, 3, 2, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(520, 1);
            panel2.TabIndex = 5;
            // 
            // loginBTN
            // 
            loginBTN.BackColor = Color.IndianRed;
            loginBTN.FlatAppearance.BorderSize = 0;
            loginBTN.FlatStyle = FlatStyle.Flat;
            loginBTN.Font = new Font("Showcard Gothic", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loginBTN.ForeColor = Color.White;
            loginBTN.Location = new Point(221, 443);
            loginBTN.Margin = new Padding(2, 3, 2, 3);
            loginBTN.Name = "loginBTN";
            loginBTN.Size = new Size(203, 45);
            loginBTN.TabIndex = 3;
            loginBTN.Text = "Log In";
            loginBTN.UseVisualStyleBackColor = false;
            loginBTN.Click += loginBTN_Click;
            // 
            // usernameTB
            // 
            usernameTB.BackColor = Color.White;
            usernameTB.BorderStyle = BorderStyle.None;
            usernameTB.Font = new Font("Microsoft YaHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            usernameTB.ForeColor = SystemColors.HotTrack;
            usernameTB.Location = new Point(182, 194);
            usernameTB.Margin = new Padding(2, 3, 2, 3);
            usernameTB.Multiline = true;
            usernameTB.Name = "usernameTB";
            usernameTB.Size = new Size(399, 43);
            usernameTB.TabIndex = 1;
            // 
            // passwordTB
            // 
            passwordTB.BackColor = Color.White;
            passwordTB.BorderStyle = BorderStyle.None;
            passwordTB.Font = new Font("Microsoft YaHei UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            passwordTB.ForeColor = SystemColors.HotTrack;
            passwordTB.Location = new Point(182, 303);
            passwordTB.Margin = new Padding(2, 3, 2, 3);
            passwordTB.Multiline = true;
            passwordTB.Name = "passwordTB";
            passwordTB.PasswordChar = '*';
            passwordTB.Size = new Size(399, 43);
            passwordTB.TabIndex = 2;
            passwordTB.KeyDown += passwordTB_KeyDown;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.BorderStyle = BorderStyle.Fixed3D;
            panel3.Controls.Add(usernameTB);
            panel3.Controls.Add(passwordTB);
            panel3.Controls.Add(pictureBox2);
            panel3.Controls.Add(panel1);
            panel3.Controls.Add(panel4);
            panel3.Controls.Add(label1);
            panel3.Controls.Add(panel2);
            panel3.Controls.Add(pictureBox3);
            panel3.Controls.Add(loginBTN);
            panel3.Location = new Point(132, 25);
            panel3.Name = "panel3";
            panel3.Size = new Size(640, 524);
            panel3.TabIndex = 6;
            // 
            // panel4
            // 
            panel4.BackColor = Color.IndianRed;
            panel4.ForeColor = SystemColors.ButtonShadow;
            panel4.Location = new Point(160, 70);
            panel4.Margin = new Padding(2, 3, 2, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(363, 1);
            panel4.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Showcard Gothic", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.IndianRed;
            label1.Location = new Point(193, 17);
            label1.Name = "label1";
            label1.Size = new Size(260, 50);
            label1.TabIndex = 0;
            label1.Text = "Ticket App";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(932, 581);
            Controls.Add(panel3);
            DoubleBuffered = true;
            Margin = new Padding(2, 3, 2, 3);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoginForm";
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private PictureBox pictureBox2;
        private Panel panel1;
        private PictureBox pictureBox3;
        private Panel panel2;
        private Button loginBTN;
        private TextBox usernameTB;
        private TextBox passwordTB;
        private Panel panel3;
        private Panel panel4;
        private Label label1;
    }
}