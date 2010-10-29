namespace Extractor
{
    partial class MainForm
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
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitFBXMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTakesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveBoneMapMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.modelViewerControl = new Extractor.ModelViewerControl();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(792, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.SystemColors.ControlDark;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openModelToolStripMenuItem,
            this.splitFBXMenuItem,
            this.openTakesToolStripMenuItem,
            this.toolStripSeparator1,
            this.SaveBoneMapMenu,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openModelToolStripMenuItem
            // 
            this.openModelToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.openModelToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.openModelToolStripMenuItem.Name = "openModelToolStripMenuItem";
            this.openModelToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.openModelToolStripMenuItem.Text = "View Model...";
            this.openModelToolStripMenuItem.ToolTipText = "Load a 3D model in to the viewer";
            this.openModelToolStripMenuItem.Click += new System.EventHandler(this.OpenModelMenuClicked);
            // 
            // splitFBXMenuItem
            // 
            this.splitFBXMenuItem.Name = "splitFBXMenuItem";
            this.splitFBXMenuItem.Size = new System.Drawing.Size(161, 22);
            this.splitFBXMenuItem.Text = "Split FBX files...";
            this.splitFBXMenuItem.ToolTipText = "Split FBX Model files to have only one take per file";
            this.splitFBXMenuItem.Click += new System.EventHandler(this.SplitFBXMenuClicked);
            // 
            // openTakesToolStripMenuItem
            // 
            this.openTakesToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.openTakesToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.openTakesToolStripMenuItem.Name = "openTakesToolStripMenuItem";
            this.openTakesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.openTakesToolStripMenuItem.Text = "Extract Takes...";
            this.openTakesToolStripMenuItem.ToolTipText = "Load a list of animation takes and save them in a keyframe format";
            this.openTakesToolStripMenuItem.Click += new System.EventHandler(this.OpenTakesMenuClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripSeparator1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
            // 
            // SaveBoneMapMenu
            // 
            this.SaveBoneMapMenu.Enabled = false;
            this.SaveBoneMapMenu.Name = "SaveBoneMapMenu";
            this.SaveBoneMapMenu.Size = new System.Drawing.Size(161, 22);
            this.SaveBoneMapMenu.Text = "Save BoneMap...";
            this.SaveBoneMapMenu.ToolTipText = "Save a list of bone names with their numeric index";
            this.SaveBoneMapMenu.Click += new System.EventHandler(this.SaveBoneMapMenuClicked);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(158, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.exitToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitMenuClicked);
            // 
            // messageBox
            // 
            this.messageBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.messageBox.Location = new System.Drawing.Point(0, 537);
            this.messageBox.Margin = new System.Windows.Forms.Padding(12);
            this.messageBox.Multiline = true;
            this.messageBox.Name = "messageBox";
            this.messageBox.ReadOnly = true;
            this.messageBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messageBox.Size = new System.Drawing.Size(792, 93);
            this.messageBox.TabIndex = 2;
            this.messageBox.TabStop = false;
            // 
            // modelViewerControl
            // 
            this.modelViewerControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.modelViewerControl.Location = new System.Drawing.Point(0, 24);
            this.modelViewerControl.Model = null;
            this.modelViewerControl.Name = "modelViewerControl";
            this.modelViewerControl.Size = new System.Drawing.Size(792, 507);
            this.modelViewerControl.TabIndex = 1;
            this.modelViewerControl.Text = "modelViewerControl";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(792, 630);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.modelViewerControl);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Take Extractor __ [John C Brown http://www.MistyManor.co.uk]";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private ModelViewerControl modelViewerControl;
        private System.Windows.Forms.ToolStripMenuItem openTakesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TextBox messageBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem SaveBoneMapMenu;
        private System.Windows.Forms.ToolStripMenuItem splitFBXMenuItem;

    }
}

