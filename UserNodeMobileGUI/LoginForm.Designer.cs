namespace UserNodeMobileGUI
{
    partial class MainForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.resetMenuItem = new System.Windows.Forms.MenuItem();
            this.loginMenuItem = new System.Windows.Forms.MenuItem();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.serviceNodeAddressLabel = new System.Windows.Forms.Label();
            this.serviceNodeAddressTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.resetMenuItem);
            this.mainMenu1.MenuItems.Add(this.loginMenuItem);
            // 
            // resetMenuItem
            // 
            this.resetMenuItem.Text = "Reset";
            this.resetMenuItem.Click += new System.EventHandler(this.resetMenuItem_Click);
            // 
            // loginMenuItem
            // 
            this.loginMenuItem.Text = "Login";
            this.loginMenuItem.Click += new System.EventHandler(this.loginMenuItem_Click);
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(3, 37);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(234, 30);
            this.usernameTextBox.TabIndex = 0;
            // 
            // usernameLabel
            // 
            this.usernameLabel.Location = new System.Drawing.Point(4, 4);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(233, 30);
            this.usernameLabel.Text = "Username";
            // 
            // passwordLabel
            // 
            this.passwordLabel.Location = new System.Drawing.Point(4, 74);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(233, 30);
            this.passwordLabel.Text = "Password";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(4, 108);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(233, 30);
            this.passwordTextBox.TabIndex = 2;
            // 
            // serviceNodeAddressLabel
            // 
            this.serviceNodeAddressLabel.Location = new System.Drawing.Point(4, 145);
            this.serviceNodeAddressLabel.Name = "serviceNodeAddressLabel";
            this.serviceNodeAddressLabel.Size = new System.Drawing.Size(233, 30);
            this.serviceNodeAddressLabel.Text = "Service Node Address";
            // 
            // serviceNodeAddressTextBox
            // 
            this.serviceNodeAddressTextBox.Location = new System.Drawing.Point(4, 179);
            this.serviceNodeAddressTextBox.Name = "serviceNodeAddressTextBox";
            this.serviceNodeAddressTextBox.Size = new System.Drawing.Size(233, 30);
            this.serviceNodeAddressTextBox.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 266);
            this.Controls.Add(this.serviceNodeAddressTextBox);
            this.Controls.Add(this.serviceNodeAddressLabel);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.usernameTextBox);
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "DarPooling Mobile Client";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.MenuItem resetMenuItem;
        private System.Windows.Forms.MenuItem loginMenuItem;
        private System.Windows.Forms.Label serviceNodeAddressLabel;
        private System.Windows.Forms.TextBox serviceNodeAddressTextBox;
    }
}

