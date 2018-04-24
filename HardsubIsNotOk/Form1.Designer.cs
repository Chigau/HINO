namespace HardsubIsNotOk
{
    partial class Form1
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Informazioni video", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Settaggi", System.Windows.Forms.HorizontalAlignment.Left);
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.extraction = new System.Windows.Forms.ProgressBar();
            this.listView1 = new System.Windows.Forms.ListView();
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.start = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSubtitlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSubtitlesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSubtitlesWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reteNeuraleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.examplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opzioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // extraction
            // 
            this.extraction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.extraction.Location = new System.Drawing.Point(12, 640);
            this.extraction.MarqueeAnimationSpeed = 10;
            this.extraction.Maximum = 1000;
            this.extraction.Name = "extraction";
            this.extraction.Size = new System.Drawing.Size(1347, 23);
            this.extraction.TabIndex = 1;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.value});
            listViewGroup1.Header = "Informazioni video";
            listViewGroup1.Name = "Informazioni video";
            listViewGroup2.Header = "Settaggi";
            listViewGroup2.Name = "Settaggi";
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(12, 372);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1346, 262);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // name
            // 
            this.name.Text = "Field";
            this.name.Width = 241;
            // 
            // value
            // 
            this.value.Text = "Value";
            this.value.Width = 635;
            // 
            // start
            // 
            this.start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.start.Enabled = false;
            this.start.Location = new System.Drawing.Point(1195, 687);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(164, 50);
            this.start.TabIndex = 4;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.reteNeuraleToolStripMenuItem,
            this.opzioniToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1370, 33);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addVideoToolStripMenuItem,
            this.saveSubtitlesToolStripMenuItem,
            this.saveSubtitlesToolStripMenuItem1,
            this.saveDictionaryToolStripMenuItem,
            this.openSubtitlesWindowToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addVideoToolStripMenuItem
            // 
            this.addVideoToolStripMenuItem.Name = "addVideoToolStripMenuItem";
            this.addVideoToolStripMenuItem.Size = new System.Drawing.Size(291, 30);
            this.addVideoToolStripMenuItem.Text = "Add video...";
            this.addVideoToolStripMenuItem.Click += new System.EventHandler(this.apriVideoToolStripMenuItem_Click);
            // 
            // saveSubtitlesToolStripMenuItem
            // 
            this.saveSubtitlesToolStripMenuItem.Name = "saveSubtitlesToolStripMenuItem";
            this.saveSubtitlesToolStripMenuItem.Size = new System.Drawing.Size(291, 30);
            this.saveSubtitlesToolStripMenuItem.Text = "Save subtitles";
            this.saveSubtitlesToolStripMenuItem.Click += new System.EventHandler(this.salvaSottotitoliToolStripMenuItem_Click_1);
            // 
            // saveSubtitlesToolStripMenuItem1
            // 
            this.saveSubtitlesToolStripMenuItem1.Name = "saveSubtitlesToolStripMenuItem1";
            this.saveSubtitlesToolStripMenuItem1.Size = new System.Drawing.Size(291, 30);
            this.saveSubtitlesToolStripMenuItem1.Text = "Change subtitles folder...";
            this.saveSubtitlesToolStripMenuItem1.Click += new System.EventHandler(this.saveSubtitlesToolStripMenuItem1_Click);
            // 
            // saveDictionaryToolStripMenuItem
            // 
            this.saveDictionaryToolStripMenuItem.Name = "saveDictionaryToolStripMenuItem";
            this.saveDictionaryToolStripMenuItem.Size = new System.Drawing.Size(291, 30);
            this.saveDictionaryToolStripMenuItem.Text = "Save dictionary";
            this.saveDictionaryToolStripMenuItem.Click += new System.EventHandler(this.salvaLeModificheAlDizionarioToolStripMenuItem_Click);
            // 
            // openSubtitlesWindowToolStripMenuItem
            // 
            this.openSubtitlesWindowToolStripMenuItem.Name = "openSubtitlesWindowToolStripMenuItem";
            this.openSubtitlesWindowToolStripMenuItem.Size = new System.Drawing.Size(291, 30);
            this.openSubtitlesWindowToolStripMenuItem.Text = "Open subtitles window";
            this.openSubtitlesWindowToolStripMenuItem.Click += new System.EventHandler(this.apriFinestraSottotitoliToolStripMenuItem_Click);
            // 
            // reteNeuraleToolStripMenuItem
            // 
            this.reteNeuraleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSettingsToolStripMenuItem,
            this.saveSettingsStripMenuItem,
            this.toolStripSeparator1,
            this.examplesToolStripMenuItem});
            this.reteNeuraleToolStripMenuItem.Name = "reteNeuraleToolStripMenuItem";
            this.reteNeuraleToolStripMenuItem.Size = new System.Drawing.Size(144, 29);
            this.reteNeuraleToolStripMenuItem.Text = "Neural network";
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(272, 30);
            this.loadSettingsToolStripMenuItem.Text = "Load settings...";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.caricaStatoToolStripMenuItem_Click);
            // 
            // saveSettingsStripMenuItem
            // 
            this.saveSettingsStripMenuItem.Name = "saveSettingsStripMenuItem";
            this.saveSettingsStripMenuItem.Size = new System.Drawing.Size(272, 30);
            this.saveSettingsStripMenuItem.Text = "Save current settings...";
            this.saveSettingsStripMenuItem.Click += new System.EventHandler(this.salvaStatoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(269, 6);
            // 
            // examplesToolStripMenuItem
            // 
            this.examplesToolStripMenuItem.Name = "examplesToolStripMenuItem";
            this.examplesToolStripMenuItem.Size = new System.Drawing.Size(272, 30);
            this.examplesToolStripMenuItem.Text = "Examples list";
            this.examplesToolStripMenuItem.Click += new System.EventHandler(this.listaEsempiToolStripMenuItem_Click);
            // 
            // opzioniToolStripMenuItem
            // 
            this.opzioniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.opzioniToolStripMenuItem.Name = "opzioniToolStripMenuItem";
            this.opzioniToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
            this.opzioniToolStripMenuItem.Text = "Options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(252, 30);
            this.settingsToolStripMenuItem.Text = "Conversion settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(13, 687);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 50);
            this.button1.TabIndex = 8;
            this.button1.Text = "Configuration";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.AllowDrop = true;
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 25;
            this.listBox1.Location = new System.Drawing.Point(13, 28);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1345, 329);
            this.listBox1.TabIndex = 9;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox1_DragDrop);
            this.listBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox1_DragEnter);
            this.listBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(22, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 25);
            this.label1.TabIndex = 10;
            this.label1.Text = "No files added yet";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 749);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.start);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.extraction);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.ProgressBar extraction;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader value;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSubtitlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reteNeuraleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem opzioniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem examplesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveDictionaryToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem openSubtitlesWindowToolStripMenuItem;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem saveSubtitlesToolStripMenuItem1;
    }
}

