namespace UserNodeSmartDeviceGUI
{
    partial class MainForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu searchTripMenu;

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
            this.searchTripMenu = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.newTripMenu = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // searchTripMenu
            // 
            this.searchTripMenu.MenuItems.Add(this.menuItem1);
            this.searchTripMenu.MenuItems.Add(this.newTripMenu);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Search Trip";
            // 
            // newTripMenu
            // 
            this.newTripMenu.Text = "New Trip";
            this.newTripMenu.Click += new System.EventHandler(this.newTripMenu_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 247);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(234, 21);
            this.label1.Text = "Status: Connected";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 20);
            this.label2.Text = "Source";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(81, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(156, 21);
            this.textBox1.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 20);
            this.label3.Text = "Destination";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(81, 58);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(156, 21);
            this.textBox2.TabIndex = 4;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(81, 30);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(100, 22);
            this.numericUpDown1.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 20);
            this.label4.Text = "+/- (km)";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(81, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 20);
            this.button1.TabIndex = 9;
            this.button1.Text = "Cancel";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(165, 245);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 20);
            this.button2.TabIndex = 14;
            this.button2.Text = "Logout";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Menu = this.searchTripMenu;
            this.Name = "MainForm";
            this.Text = "Darpooling - Connected";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem newTripMenu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;

    }
}