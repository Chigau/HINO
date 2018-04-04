namespace HardsubIsNotOk
{
    partial class SubtitlesWindow
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.modificaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inserisciToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sincronizzaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aggiornaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SubtitlesTable = new System.Windows.Forms.ListView();
            this.Text = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Start = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.End = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modificaToolStripMenuItem,
            this.aggiornaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1015, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // modificaToolStripMenuItem
            // 
            this.modificaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inserisciToolStripMenuItem,
            this.sincronizzaToolStripMenuItem});
            this.modificaToolStripMenuItem.Name = "modificaToolStripMenuItem";
            this.modificaToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.modificaToolStripMenuItem.Text = "Edit";
            // 
            // inserisciToolStripMenuItem
            // 
            this.inserisciToolStripMenuItem.Name = "inserisciToolStripMenuItem";
            this.inserisciToolStripMenuItem.Size = new System.Drawing.Size(210, 30);
            this.inserisciToolStripMenuItem.Text = "Insert...";
            // 
            // sincronizzaToolStripMenuItem
            // 
            this.sincronizzaToolStripMenuItem.Name = "sincronizzaToolStripMenuItem";
            this.sincronizzaToolStripMenuItem.Size = new System.Drawing.Size(210, 30);
            this.sincronizzaToolStripMenuItem.Text = "Synchronize...";
            // 
            // aggiornaToolStripMenuItem
            // 
            this.aggiornaToolStripMenuItem.Name = "aggiornaToolStripMenuItem";
            this.aggiornaToolStripMenuItem.Size = new System.Drawing.Size(82, 29);
            this.aggiornaToolStripMenuItem.Text = "Refresh";
            this.aggiornaToolStripMenuItem.Click += new System.EventHandler(this.aggiornaToolStripMenuItem_Click);
            // 
            // SubtitlesTable
            // 
            this.SubtitlesTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SubtitlesTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Text,
            this.Start,
            this.End});
            this.SubtitlesTable.GridLines = true;
            this.SubtitlesTable.LabelEdit = true;
            this.SubtitlesTable.Location = new System.Drawing.Point(12, 36);
            this.SubtitlesTable.Name = "SubtitlesTable";
            this.SubtitlesTable.ShowGroups = false;
            this.SubtitlesTable.ShowItemToolTips = true;
            this.SubtitlesTable.Size = new System.Drawing.Size(991, 915);
            this.SubtitlesTable.TabIndex = 1;
            this.SubtitlesTable.UseCompatibleStateImageBehavior = false;
            this.SubtitlesTable.View = System.Windows.Forms.View.Details;
            // 
            // Text
            // 
            this.Text.Text = "Text";
            this.Text.Width = 656;
            // 
            // Start
            // 
            this.Start.Text = "Start";
            this.Start.Width = 114;
            // 
            // End
            // 
            this.End.Text = "End";
            this.End.Width = 193;
            // 
            // SubtitlesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 963);
            this.Controls.Add(this.SubtitlesTable);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SubtitlesWindow";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem modificaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inserisciToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sincronizzaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aggiornaToolStripMenuItem;
        private System.Windows.Forms.ListView SubtitlesTable;
        private System.Windows.Forms.ColumnHeader Text;
        private System.Windows.Forms.ColumnHeader Start;
        private System.Windows.Forms.ColumnHeader End;
    }
}