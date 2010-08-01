namespace UserNodeGUI
{
    partial class ConnectDialog
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
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.UsernameTextbox = new System.Windows.Forms.TextBox();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.AccountGroupBox = new System.Windows.Forms.GroupBox();
            this.ProviderTextBox = new System.Windows.Forms.TextBox();
            this.ProviderLabel = new System.Windows.Forms.Label();
            this.ServiceGroupBox = new System.Windows.Forms.GroupBox();
            this.FavouriteCheckBox = new System.Windows.Forms.CheckBox();
            this.AddressComboBox = new System.Windows.Forms.ComboBox();
            this.AccountGroupBox.SuspendLayout();
            this.ServiceGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(12, 22);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(55, 13);
            this.UsernameLabel.TabIndex = 0;
            this.UsernameLabel.Text = "Username";
            // 
            // UsernameTextbox
            // 
            this.UsernameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.UsernameTextbox.Location = new System.Drawing.Point(78, 19);
            this.UsernameTextbox.Name = "UsernameTextbox";
            this.UsernameTextbox.Size = new System.Drawing.Size(255, 20);
            this.UsernameTextbox.TabIndex = 1;
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(12, 48);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(53, 13);
            this.PasswordLabel.TabIndex = 2;
            this.PasswordLabel.Text = "Password";
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PasswordTextBox.Location = new System.Drawing.Point(78, 45);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.Size = new System.Drawing.Size(255, 20);
            this.PasswordTextBox.TabIndex = 3;
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Location = new System.Drawing.Point(12, 22);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(45, 13);
            this.AddressLabel.TabIndex = 5;
            this.AddressLabel.Text = "Address";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnectButton.Location = new System.Drawing.Point(201, 238);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 6;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(282, 238);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 7;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // AccountGroupBox
            // 
            this.AccountGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AccountGroupBox.Controls.Add(this.ProviderTextBox);
            this.AccountGroupBox.Controls.Add(this.ProviderLabel);
            this.AccountGroupBox.Controls.Add(this.UsernameLabel);
            this.AccountGroupBox.Controls.Add(this.UsernameTextbox);
            this.AccountGroupBox.Controls.Add(this.PasswordTextBox);
            this.AccountGroupBox.Controls.Add(this.PasswordLabel);
            this.AccountGroupBox.Location = new System.Drawing.Point(12, 12);
            this.AccountGroupBox.Name = "AccountGroupBox";
            this.AccountGroupBox.Size = new System.Drawing.Size(345, 115);
            this.AccountGroupBox.TabIndex = 8;
            this.AccountGroupBox.TabStop = false;
            this.AccountGroupBox.Text = "Account Details";
            // 
            // ProviderTextBox
            // 
            this.ProviderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProviderTextBox.Location = new System.Drawing.Point(78, 72);
            this.ProviderTextBox.Name = "ProviderTextBox";
            this.ProviderTextBox.Size = new System.Drawing.Size(255, 20);
            this.ProviderTextBox.TabIndex = 5;
            // 
            // ProviderLabel
            // 
            this.ProviderLabel.AutoSize = true;
            this.ProviderLabel.Location = new System.Drawing.Point(12, 75);
            this.ProviderLabel.Name = "ProviderLabel";
            this.ProviderLabel.Size = new System.Drawing.Size(46, 13);
            this.ProviderLabel.TabIndex = 4;
            this.ProviderLabel.Text = "Provider";
            // 
            // ServiceGroupBox
            // 
            this.ServiceGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ServiceGroupBox.Controls.Add(this.FavouriteCheckBox);
            this.ServiceGroupBox.Controls.Add(this.AddressComboBox);
            this.ServiceGroupBox.Controls.Add(this.AddressLabel);
            this.ServiceGroupBox.Location = new System.Drawing.Point(12, 133);
            this.ServiceGroupBox.Name = "ServiceGroupBox";
            this.ServiceGroupBox.Size = new System.Drawing.Size(345, 72);
            this.ServiceGroupBox.TabIndex = 9;
            this.ServiceGroupBox.TabStop = false;
            this.ServiceGroupBox.Text = "ServiceNode";
            // 
            // FavouriteCheckBox
            // 
            this.FavouriteCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FavouriteCheckBox.AutoSize = true;
            this.FavouriteCheckBox.Location = new System.Drawing.Point(238, 46);
            this.FavouriteCheckBox.Name = "FavouriteCheckBox";
            this.FavouriteCheckBox.Size = new System.Drawing.Size(101, 17);
            this.FavouriteCheckBox.TabIndex = 11;
            this.FavouriteCheckBox.Text = "Add to favourite";
            this.FavouriteCheckBox.UseVisualStyleBackColor = true;
            // 
            // AddressComboBox
            // 
            this.AddressComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AddressComboBox.FormattingEnabled = true;
            this.AddressComboBox.Location = new System.Drawing.Point(78, 19);
            this.AddressComboBox.Name = "AddressComboBox";
            this.AddressComboBox.Size = new System.Drawing.Size(255, 21);
            this.AddressComboBox.TabIndex = 10;
            // 
            // ConnectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 273);
            this.Controls.Add(this.ServiceGroupBox);
            this.Controls.Add(this.AccountGroupBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ConnectButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectDialog";
            this.Text = "ConnectDialog";
            this.Load += new System.EventHandler(this.ConnectDialog_Load);
            this.AccountGroupBox.ResumeLayout(false);
            this.AccountGroupBox.PerformLayout();
            this.ServiceGroupBox.ResumeLayout(false);
            this.ServiceGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.TextBox UsernameTextbox;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Label AddressLabel;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.GroupBox AccountGroupBox;
        private System.Windows.Forms.GroupBox ServiceGroupBox;
        private System.Windows.Forms.ComboBox AddressComboBox;
        private System.Windows.Forms.CheckBox FavouriteCheckBox;
        private System.Windows.Forms.TextBox ProviderTextBox;
        private System.Windows.Forms.Label ProviderLabel;
    }
}