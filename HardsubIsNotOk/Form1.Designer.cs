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
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Informazioni video", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Settaggi", System.Windows.Forms.HorizontalAlignment.Left);
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.extraction = new System.Windows.Forms.ProgressBar();
            this.listView1 = new System.Windows.Forms.ListView();
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.start = new System.Windows.Forms.Button();
            this.recognition = new System.Windows.Forms.ProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caricaVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.apriFinestraSottotitoliToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvaSottotitoliToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvaLeModificheAlDizionarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reteNeuraleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caricaStatoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvaStatoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.listaEsempiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opzioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
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
            this.extraction.Location = new System.Drawing.Point(12, 1147);
            this.extraction.MarqueeAnimationSpeed = 10;
            this.extraction.Maximum = 1000;
            this.extraction.Name = "extraction";
            this.extraction.Size = new System.Drawing.Size(2454, 23);
            this.extraction.TabIndex = 1;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.value});
            listViewGroup5.Header = "Informazioni video";
            listViewGroup5.Name = "Informazioni video";
            listViewGroup6.Header = "Settaggi";
            listViewGroup6.Name = "Settaggi";
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup5,
            listViewGroup6});
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(12, 36);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(2454, 800);
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
            this.start.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.start.Enabled = false;
            this.start.Location = new System.Drawing.Point(1255, 1059);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(164, 50);
            this.start.TabIndex = 4;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // recognition
            // 
            this.recognition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recognition.Location = new System.Drawing.Point(12, 1196);
            this.recognition.MarqueeAnimationSpeed = 10;
            this.recognition.Maximum = 1000;
            this.recognition.Name = "recognition";
            this.recognition.Size = new System.Drawing.Size(2454, 23);
            this.recognition.TabIndex = 5;
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
            this.menuStrip1.Size = new System.Drawing.Size(2478, 33);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.caricaVideoToolStripMenuItem,
            this.apriFinestraSottotitoliToolStripMenuItem,
            this.salvaSottotitoliToolStripMenuItem,
            this.salvaLeModificheAlDizionarioToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // caricaVideoToolStripMenuItem
            // 
            this.caricaVideoToolStripMenuItem.Name = "caricaVideoToolStripMenuItem";
            this.caricaVideoToolStripMenuItem.Size = new System.Drawing.Size(286, 30);
            this.caricaVideoToolStripMenuItem.Text = "Load video...";
            this.caricaVideoToolStripMenuItem.Click += new System.EventHandler(this.apriVideoToolStripMenuItem_Click);
            // 
            // apriFinestraSottotitoliToolStripMenuItem
            // 
            this.apriFinestraSottotitoliToolStripMenuItem.Name = "apriFinestraSottotitoliToolStripMenuItem";
            this.apriFinestraSottotitoliToolStripMenuItem.Size = new System.Drawing.Size(286, 30);
            this.apriFinestraSottotitoliToolStripMenuItem.Text = "Open subtitles windows";
            this.apriFinestraSottotitoliToolStripMenuItem.Click += new System.EventHandler(this.apriFinestraSottotitoliToolStripMenuItem_Click);
            // 
            // salvaSottotitoliToolStripMenuItem
            // 
            this.salvaSottotitoliToolStripMenuItem.Name = "salvaSottotitoliToolStripMenuItem";
            this.salvaSottotitoliToolStripMenuItem.Size = new System.Drawing.Size(286, 30);
            this.salvaSottotitoliToolStripMenuItem.Text = "Save subtitles...";
            this.salvaSottotitoliToolStripMenuItem.Click += new System.EventHandler(this.salvaSottotitoliToolStripMenuItem_Click_1);
            // 
            // salvaLeModificheAlDizionarioToolStripMenuItem
            // 
            this.salvaLeModificheAlDizionarioToolStripMenuItem.Name = "salvaLeModificheAlDizionarioToolStripMenuItem";
            this.salvaLeModificheAlDizionarioToolStripMenuItem.Size = new System.Drawing.Size(286, 30);
            this.salvaLeModificheAlDizionarioToolStripMenuItem.Text = "Save dictionary";
            this.salvaLeModificheAlDizionarioToolStripMenuItem.Click += new System.EventHandler(this.salvaLeModificheAlDizionarioToolStripMenuItem_Click);
            // 
            // reteNeuraleToolStripMenuItem
            // 
            this.reteNeuraleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.caricaStatoToolStripMenuItem,
            this.salvaStatoToolStripMenuItem,
            this.toolStripSeparator1,
            this.listaEsempiToolStripMenuItem});
            this.reteNeuraleToolStripMenuItem.Name = "reteNeuraleToolStripMenuItem";
            this.reteNeuraleToolStripMenuItem.Size = new System.Drawing.Size(144, 29);
            this.reteNeuraleToolStripMenuItem.Text = "Neural network";
            // 
            // caricaStatoToolStripMenuItem
            // 
            this.caricaStatoToolStripMenuItem.Name = "caricaStatoToolStripMenuItem";
            this.caricaStatoToolStripMenuItem.Size = new System.Drawing.Size(272, 30);
            this.caricaStatoToolStripMenuItem.Text = "Load settings...";
            this.caricaStatoToolStripMenuItem.Click += new System.EventHandler(this.caricaStatoToolStripMenuItem_Click);
            // 
            // salvaStatoToolStripMenuItem
            // 
            this.salvaStatoToolStripMenuItem.Name = "salvaStatoToolStripMenuItem";
            this.salvaStatoToolStripMenuItem.Size = new System.Drawing.Size(272, 30);
            this.salvaStatoToolStripMenuItem.Text = "Save current settings...";
            this.salvaStatoToolStripMenuItem.Click += new System.EventHandler(this.salvaStatoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(269, 6);
            // 
            // listaEsempiToolStripMenuItem
            // 
            this.listaEsempiToolStripMenuItem.Name = "listaEsempiToolStripMenuItem";
            this.listaEsempiToolStripMenuItem.Size = new System.Drawing.Size(272, 30);
            this.listaEsempiToolStripMenuItem.Text = "Examples list";
            this.listaEsempiToolStripMenuItem.Click += new System.EventHandler(this.listaEsempiToolStripMenuItem_Click);
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
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(1086, 1059);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 50);
            this.button1.TabIndex = 8;
            this.button1.Text = "Configuration";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2478, 1244);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.recognition);
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
        private System.Windows.Forms.ProgressBar recognition;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem caricaVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvaSottotitoliToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reteNeuraleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem caricaStatoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvaStatoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem opzioniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listaEsempiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvaLeModificheAlDizionarioToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem apriFinestraSottotitoliToolStripMenuItem;
    }
}

