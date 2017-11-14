namespace AsniZX
{
    partial class ZXForm
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
            this.MainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSnapshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleFullscreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x51280x960ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x61536x1152ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.togglePauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terminateEmulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.softResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenuStrip
            // 
            this.MainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.controlToolStripMenuItem});
            this.MainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip.Name = "MainMenuStrip";
            this.MainMenuStrip.Size = new System.Drawing.Size(623, 24);
            this.MainMenuStrip.TabIndex = 0;
            this.MainMenuStrip.Text = "MainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSnapshotToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadSnapshotToolStripMenuItem
            // 
            this.loadSnapshotToolStripMenuItem.Name = "loadSnapshotToolStripMenuItem";
            this.loadSnapshotToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadSnapshotToolStripMenuItem.Text = "Load Snapshot";
            this.loadSnapshotToolStripMenuItem.Click += new System.EventHandler(this.loadSnapshotToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleFullscreenToolStripMenuItem,
            this.windowSizeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // toggleFullscreenToolStripMenuItem
            // 
            this.toggleFullscreenToolStripMenuItem.Name = "toggleFullscreenToolStripMenuItem";
            this.toggleFullscreenToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.toggleFullscreenToolStripMenuItem.Text = "Toggle Fullscreen (F12)";
            this.toggleFullscreenToolStripMenuItem.Click += new System.EventHandler(this.toggleFullscreenToolStripMenuItem_Click);
            // 
            // windowSizeToolStripMenuItem
            // 
            this.windowSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actualToolStripMenuItem,
            this.x2ToolStripMenuItem,
            this.x3ToolStripMenuItem,
            this.x4ToolStripMenuItem,
            this.x51280x960ToolStripMenuItem,
            this.x61536x1152ToolStripMenuItem});
            this.windowSizeToolStripMenuItem.Name = "windowSizeToolStripMenuItem";
            this.windowSizeToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.windowSizeToolStripMenuItem.Text = "Spectrum Screen Size";
            // 
            // actualToolStripMenuItem
            // 
            this.actualToolStripMenuItem.Name = "actualToolStripMenuItem";
            this.actualToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.actualToolStripMenuItem.Text = "Actual (256x192)";
            this.actualToolStripMenuItem.Click += new System.EventHandler(this.actualToolStripMenuItem_Click);
            // 
            // x2ToolStripMenuItem
            // 
            this.x2ToolStripMenuItem.Name = "x2ToolStripMenuItem";
            this.x2ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.x2ToolStripMenuItem.Text = "x2 (512x284)";
            this.x2ToolStripMenuItem.Click += new System.EventHandler(this.x2ToolStripMenuItem_Click);
            // 
            // x3ToolStripMenuItem
            // 
            this.x3ToolStripMenuItem.Name = "x3ToolStripMenuItem";
            this.x3ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.x3ToolStripMenuItem.Text = "x3 (768x576)";
            this.x3ToolStripMenuItem.Click += new System.EventHandler(this.x3ToolStripMenuItem_Click);
            // 
            // x4ToolStripMenuItem
            // 
            this.x4ToolStripMenuItem.Name = "x4ToolStripMenuItem";
            this.x4ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.x4ToolStripMenuItem.Text = "x4 (1024x768)";
            this.x4ToolStripMenuItem.Click += new System.EventHandler(this.x4ToolStripMenuItem_Click);
            // 
            // x51280x960ToolStripMenuItem
            // 
            this.x51280x960ToolStripMenuItem.Name = "x51280x960ToolStripMenuItem";
            this.x51280x960ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.x51280x960ToolStripMenuItem.Text = "x5 (1280x960)";
            this.x51280x960ToolStripMenuItem.Click += new System.EventHandler(this.x51280x960ToolStripMenuItem_Click);
            // 
            // x61536x1152ToolStripMenuItem
            // 
            this.x61536x1152ToolStripMenuItem.Name = "x61536x1152ToolStripMenuItem";
            this.x61536x1152ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.x61536x1152ToolStripMenuItem.Text = "x6 (1536x1152)";
            this.x61536x1152ToolStripMenuItem.Click += new System.EventHandler(this.x61536x1152ToolStripMenuItem_Click);
            // 
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.togglePauseToolStripMenuItem,
            this.terminateEmulationToolStripMenuItem,
            this.softResetToolStripMenuItem});
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.controlToolStripMenuItem.Text = "Control";
            // 
            // togglePauseToolStripMenuItem
            // 
            this.togglePauseToolStripMenuItem.Name = "togglePauseToolStripMenuItem";
            this.togglePauseToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.togglePauseToolStripMenuItem.Text = "Toggle Pause";
            this.togglePauseToolStripMenuItem.Click += new System.EventHandler(this.togglePauseToolStripMenuItem_Click);
            // 
            // terminateEmulationToolStripMenuItem
            // 
            this.terminateEmulationToolStripMenuItem.Name = "terminateEmulationToolStripMenuItem";
            this.terminateEmulationToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.terminateEmulationToolStripMenuItem.Text = "Hard Reset";
            this.terminateEmulationToolStripMenuItem.Click += new System.EventHandler(this.terminateEmulationToolStripMenuItem_Click);
            // 
            // softResetToolStripMenuItem
            // 
            this.softResetToolStripMenuItem.Name = "softResetToolStripMenuItem";
            this.softResetToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.softResetToolStripMenuItem.Text = "Soft Reset";
            this.softResetToolStripMenuItem.Click += new System.EventHandler(this.softResetToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 496);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(623, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Click += new System.EventHandler(this.toolStripStatusLabel2_Click);
            // 
            // ZXForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(623, 518);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.MainMenuStrip);
            this.KeyPreview = true;
            this.Name = "ZXForm";
            this.ShowIcon = false;
            this.Text = "AsniZX - Terrible Speccy Emulation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ZXForm_FormClosing);
            this.Load += new System.EventHandler(this.ZXForm_Load);
            this.ResizeBegin += new System.EventHandler(this.ZXForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.ZXForm_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ZXForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ZXForm_KeyUp);
            this.Resize += new System.EventHandler(this.ZXForm_Resize);
            this.MainMenuStrip.ResumeLayout(false);
            this.MainMenuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleFullscreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x51280x960ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x61536x1152ToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem togglePauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem terminateEmulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem softResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSnapshotToolStripMenuItem;
    }
}