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
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            panel1 = new Panel();
            pictureBox3 = new PictureBox();
            panel2 = new Panel();
            loginBTN = new Button();
            usernameTB = new TextBox();
            passwordTB = new TextBox();
            exitAppBTN = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.logo_image;
            pictureBox1.Location = new Point(1, -1);
            pictureBox1.Margin = new Padding(2, 3, 2, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(560, 491);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.red_user_icon;
            pictureBox2.Location = new Point(602, 155);
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
            panel1.Location = new Point(602, 203);
            panel1.Margin = new Padding(2, 3, 2, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(363, 3);
            panel1.TabIndex = 5;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.security_icon;
            pictureBox3.Location = new Point(602, 251);
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
            panel2.Location = new Point(602, 299);
            panel2.Margin = new Padding(2, 3, 2, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(363, 3);
            panel2.TabIndex = 5;
            // 
            // loginBTN
            // 
            loginBTN.BackColor = Color.IndianRed;
            loginBTN.FlatAppearance.BorderSize = 0;
            loginBTN.FlatStyle = FlatStyle.Flat;
            loginBTN.Font = new Font("Showcard Gothic", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loginBTN.ForeColor = Color.White;
            loginBTN.Location = new Point(673, 315);
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
            usernameTB.Location = new Point(732, 155);
            usernameTB.Margin = new Padding(2, 3, 2, 3);
            usernameTB.Multiline = true;
            usernameTB.Name = "usernameTB";
            usernameTB.Size = new Size(234, 43);
            usernameTB.TabIndex = 1;
            // 
            // passwordTB
            // 
            passwordTB.BackColor = Color.White;
            passwordTB.BorderStyle = BorderStyle.None;
            passwordTB.Font = new Font("Microsoft YaHei UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            passwordTB.ForeColor = SystemColors.HotTrack;
            passwordTB.Location = new Point(737, 251);
            passwordTB.Margin = new Padding(2, 3, 2, 3);
            passwordTB.Multiline = true;
            passwordTB.Name = "passwordTB";
            passwordTB.PasswordChar = '*';
            passwordTB.Size = new Size(229, 43);
            passwordTB.TabIndex = 2;
            // 
            // exitAppBTN
            // 
            exitAppBTN.BackgroundImage = Properties.Resources.close_form_icon;
            exitAppBTN.BackgroundImageLayout = ImageLayout.Stretch;
            exitAppBTN.FlatAppearance.BorderSize = 0;
            exitAppBTN.FlatStyle = FlatStyle.Flat;
            exitAppBTN.Location = new Point(993, 9);
            exitAppBTN.Margin = new Padding(2, 3, 2, 3);
            exitAppBTN.Name = "exitAppBTN";
            exitAppBTN.Size = new Size(43, 43);
            exitAppBTN.TabIndex = 6;
            exitAppBTN.UseVisualStyleBackColor = true;
            exitAppBTN.Click += exitAppBTN_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1047, 492);
            Controls.Add(exitAppBTN);
            Controls.Add(passwordTB);
            Controls.Add(usernameTB);
            Controls.Add(loginBTN);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(2, 3, 2, 3);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoginForm";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Panel panel1;
        private PictureBox pictureBox3;
        private Panel panel2;
        private Button loginBTN;
        private TextBox usernameTB;
        private TextBox passwordTB;
        private Button exitAppBTN;
    }
}