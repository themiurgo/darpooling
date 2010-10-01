using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UserNodeGUI
{
    public partial class ResultTabPage : TabPage
    {
        private DataGridView gridview;

        public ResultTabPage()
        {
            InitializeComponent();
            
            gridview = new DataGridView();
            gridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridview.Dock = System.Windows.Forms.DockStyle.Fill;
            gridview.Location = new System.Drawing.Point(3, 3);
            gridview.Name = "resultGridView";
            gridview.TabIndex = 0;

            Controls.Add(gridview);
            Location = new System.Drawing.Point(4, 22);
            Padding = new System.Windows.Forms.Padding(3);
            Size = new System.Drawing.Size(422, 379);
            TabIndex = 0;
            UseVisualStyleBackColor = true;
        }

        public void setGridviewDatasource(object source)
        {
            gridview.DataSource = source;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
