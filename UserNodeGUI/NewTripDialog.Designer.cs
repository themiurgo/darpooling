namespace UserNodeGUI
{
    partial class NewTripDialog
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
            this.departureNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.arrivalNameTextBox = new System.Windows.Forms.TextBox();
            this.freeSeatsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.smokingCheckBox = new System.Windows.Forms.CheckBox();
            this.musicCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.departureDatePicker = new System.Windows.Forms.DateTimePicker();
            this.departureTimePicker = new System.Windows.Forms.DateTimePicker();
            this.arrivalDatePicker = new System.Windows.Forms.DateTimePicker();
            this.arrivalTimePicker = new System.Windows.Forms.DateTimePicker();
            this.CancButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.contributeUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.notesTextBox = new System.Windows.Forms.TextBox();
            this.notesLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.freeSeatsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contributeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // departureNameTextBox
            // 
            this.departureNameTextBox.Location = new System.Drawing.Point(72, 6);
            this.departureNameTextBox.Name = "departureNameTextBox";
            this.departureNameTextBox.Size = new System.Drawing.Size(309, 20);
            this.departureNameTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Departure";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Arrival";
            // 
            // arrivalNameTextBox
            // 
            this.arrivalNameTextBox.Location = new System.Drawing.Point(72, 58);
            this.arrivalNameTextBox.Name = "arrivalNameTextBox";
            this.arrivalNameTextBox.Size = new System.Drawing.Size(309, 20);
            this.arrivalNameTextBox.TabIndex = 3;
            // 
            // freeSeatsUpDown
            // 
            this.freeSeatsUpDown.Location = new System.Drawing.Point(72, 119);
            this.freeSeatsUpDown.Name = "freeSeatsUpDown";
            this.freeSeatsUpDown.Size = new System.Drawing.Size(43, 20);
            this.freeSeatsUpDown.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Free seats";
            // 
            // smokingCheckBox
            // 
            this.smokingCheckBox.AutoSize = true;
            this.smokingCheckBox.Location = new System.Drawing.Point(253, 147);
            this.smokingCheckBox.Name = "smokingCheckBox";
            this.smokingCheckBox.Size = new System.Drawing.Size(67, 17);
            this.smokingCheckBox.TabIndex = 8;
            this.smokingCheckBox.Text = "Smoking";
            this.smokingCheckBox.UseVisualStyleBackColor = true;
            // 
            // musicCheckBox
            // 
            this.musicCheckBox.AutoSize = true;
            this.musicCheckBox.Location = new System.Drawing.Point(325, 147);
            this.musicCheckBox.Name = "musicCheckBox";
            this.musicCheckBox.Size = new System.Drawing.Size(54, 17);
            this.musicCheckBox.TabIndex = 9;
            this.musicCheckBox.Text = "Music";
            this.musicCheckBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Contribute";
            // 
            // departureDatePicker
            // 
            this.departureDatePicker.Location = new System.Drawing.Point(72, 32);
            this.departureDatePicker.Name = "departureDatePicker";
            this.departureDatePicker.Size = new System.Drawing.Size(200, 20);
            this.departureDatePicker.TabIndex = 1;
            // 
            // departureTimePicker
            // 
            this.departureTimePicker.CustomFormat = "HH:mm";
            this.departureTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.departureTimePicker.Location = new System.Drawing.Point(278, 32);
            this.departureTimePicker.Name = "departureTimePicker";
            this.departureTimePicker.ShowUpDown = true;
            this.departureTimePicker.Size = new System.Drawing.Size(55, 20);
            this.departureTimePicker.TabIndex = 2;
            // 
            // arrivalDatePicker
            // 
            this.arrivalDatePicker.Location = new System.Drawing.Point(72, 84);
            this.arrivalDatePicker.Name = "arrivalDatePicker";
            this.arrivalDatePicker.Size = new System.Drawing.Size(200, 20);
            this.arrivalDatePicker.TabIndex = 4;
            // 
            // arrivalTimePicker
            // 
            this.arrivalTimePicker.CustomFormat = "HH:mm";
            this.arrivalTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.arrivalTimePicker.Location = new System.Drawing.Point(278, 84);
            this.arrivalTimePicker.Name = "arrivalTimePicker";
            this.arrivalTimePicker.ShowUpDown = true;
            this.arrivalTimePicker.Size = new System.Drawing.Size(55, 20);
            this.arrivalTimePicker.TabIndex = 5;
            // 
            // CancButton
            // 
            this.CancButton.Location = new System.Drawing.Point(305, 278);
            this.CancButton.Name = "CancButton";
            this.CancButton.Size = new System.Drawing.Size(75, 23);
            this.CancButton.TabIndex = 14;
            this.CancButton.Text = "Cancel";
            this.CancButton.UseVisualStyleBackColor = true;
            this.CancButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(224, 278);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 15;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // contributeUpDown
            // 
            this.contributeUpDown.Location = new System.Drawing.Point(72, 146);
            this.contributeUpDown.Name = "contributeUpDown";
            this.contributeUpDown.Size = new System.Drawing.Size(43, 20);
            this.contributeUpDown.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(121, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "EUR";
            // 
            // notesTextBox
            // 
            this.notesTextBox.Location = new System.Drawing.Point(15, 188);
            this.notesTextBox.MaxLength = 1000;
            this.notesTextBox.Multiline = true;
            this.notesTextBox.Name = "notesTextBox";
            this.notesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.notesTextBox.Size = new System.Drawing.Size(366, 84);
            this.notesTextBox.TabIndex = 18;
            // 
            // notesLabel
            // 
            this.notesLabel.AutoSize = true;
            this.notesLabel.Location = new System.Drawing.Point(12, 172);
            this.notesLabel.Name = "notesLabel";
            this.notesLabel.Size = new System.Drawing.Size(114, 13);
            this.notesLabel.TabIndex = 19;
            this.notesLabel.Text = "Notes (max 1000 char)";
            // 
            // NewTripDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 313);
            this.Controls.Add(this.notesLabel);
            this.Controls.Add(this.notesTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.contributeUpDown);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancButton);
            this.Controls.Add(this.arrivalTimePicker);
            this.Controls.Add(this.arrivalDatePicker);
            this.Controls.Add(this.departureTimePicker);
            this.Controls.Add(this.departureDatePicker);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.musicCheckBox);
            this.Controls.Add(this.smokingCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.freeSeatsUpDown);
            this.Controls.Add(this.arrivalNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.departureNameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NewTripDialog";
            this.Text = "New Trip";
            ((System.ComponentModel.ISupportInitialize)(this.freeSeatsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contributeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox departureNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox arrivalNameTextBox;
        private System.Windows.Forms.NumericUpDown freeSeatsUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox smokingCheckBox;
        private System.Windows.Forms.CheckBox musicCheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker departureDatePicker;
        private System.Windows.Forms.DateTimePicker departureTimePicker;
        private System.Windows.Forms.DateTimePicker arrivalDatePicker;
        private System.Windows.Forms.DateTimePicker arrivalTimePicker;
        private System.Windows.Forms.Button CancButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.NumericUpDown contributeUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox notesTextBox;
        private System.Windows.Forms.Label notesLabel;
    }
}