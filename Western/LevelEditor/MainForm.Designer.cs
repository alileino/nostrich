namespace LevelEditor
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
            this._statusBar = new System.Windows.Forms.StatusStrip();
            this.tileCoordinates = new System.Windows.Forms.ToolStripStatusLabel();
            this.layerNumber = new System.Windows.Forms.ToolStripStatusLabel();
            this.mode = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this._tileFileList = new System.Windows.Forms.ListView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this._tileSelector = new System.Windows.Forms.ListView();
            this._sideTabs = new System.Windows.Forms.TabControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.padToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customGLControl1 = new LevelEditor.CustomGLControl();
            this._statusBar.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this._sideTabs.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _statusBar
            // 
            this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileCoordinates,
            this.layerNumber,
            this.mode});
            this._statusBar.Location = new System.Drawing.Point(0, 380);
            this._statusBar.Name = "_statusBar";
            this._statusBar.Size = new System.Drawing.Size(756, 22);
            this._statusBar.TabIndex = 2;
            this._statusBar.Text = "statusStrip1";
            // 
            // tileCoordinates
            // 
            this.tileCoordinates.Name = "tileCoordinates";
            this.tileCoordinates.Size = new System.Drawing.Size(22, 17);
            this.tileCoordinates.Text = "0,0";
            // 
            // layerNumber
            // 
            this.layerNumber.Name = "layerNumber";
            this.layerNumber.Size = new System.Drawing.Size(47, 17);
            this.layerNumber.Text = "Layer: 1";
            // 
            // mode
            // 
            this.mode.Name = "mode";
            this.mode.Size = new System.Drawing.Size(71, 17);
            this.mode.Text = "Mode: Paint";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this._tileFileList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(155, 325);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tilesets";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // _tileFileList
            // 
            this._tileFileList.Location = new System.Drawing.Point(0, 0);
            this._tileFileList.MultiSelect = false;
            this._tileFileList.Name = "_tileFileList";
            this._tileFileList.Size = new System.Drawing.Size(155, 319);
            this._tileFileList.TabIndex = 0;
            this._tileFileList.UseCompatibleStateImageBehavior = false;
            this._tileFileList.View = System.Windows.Forms.View.List;
            this._tileFileList.ItemActivate += new System.EventHandler(this.tileFileList_SelectedIndexChanged);
            this._tileFileList.SelectedIndexChanged += new System.EventHandler(this.tileFileList_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._tileSelector);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(155, 325);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tiles";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // _tileSelector
            // 
            this._tileSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._tileSelector.HideSelection = false;
            this._tileSelector.Location = new System.Drawing.Point(3, 3);
            this._tileSelector.Margin = new System.Windows.Forms.Padding(0);
            this._tileSelector.MultiSelect = false;
            this._tileSelector.Name = "_tileSelector";
            this._tileSelector.Size = new System.Drawing.Size(149, 322);
            this._tileSelector.TabIndex = 1;
            this._tileSelector.TileSize = new System.Drawing.Size(32, 32);
            this._tileSelector.UseCompatibleStateImageBehavior = false;
            this._tileSelector.SelectedIndexChanged += new System.EventHandler(this._tileSelector_SelectedIndexChanged);
            // 
            // _sideTabs
            // 
            this._sideTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._sideTabs.Controls.Add(this.tabPage1);
            this._sideTabs.Controls.Add(this.tabPage2);
            this._sideTabs.Location = new System.Drawing.Point(0, 29);
            this._sideTabs.Name = "_sideTabs";
            this._sideTabs.SelectedIndex = 0;
            this._sideTabs.Size = new System.Drawing.Size(163, 351);
            this._sideTabs.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(756, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.padToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // padToolStripMenuItem
            // 
            this.padToolStripMenuItem.Name = "padToolStripMenuItem";
            this.padToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.padToolStripMenuItem.Text = "Pad";
            this.padToolStripMenuItem.Click += new System.EventHandler(this.padToolStripMenuItem_Click);
            // 
            // customGLControl1
            // 
            this.customGLControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customGLControl1.AutoSize = true;
            this.customGLControl1.BackColor = System.Drawing.Color.Black;
            this.customGLControl1.ClearColor = System.Drawing.Color.Fuchsia;
            this.customGLControl1.CollisionNum = 0;
            this.customGLControl1.Location = new System.Drawing.Point(166, 24);
            this.customGLControl1.Margin = new System.Windows.Forms.Padding(0);
            this.customGLControl1.MaximumSize = new System.Drawing.Size(2000, 2000);
            this.customGLControl1.Name = "customGLControl1";
            this.customGLControl1.SelectedLayer = 0;
            this.customGLControl1.Size = new System.Drawing.Size(590, 356);
            this.customGLControl1.TabIndex = 0;
            this.customGLControl1.VSync = false;
            this.customGLControl1.MouseOverTile += new LevelEditor.CustomGLControl.MouseOverTileChangedHandler(this.customGLControl1_MouseOverTile);
            this.customGLControl1.Load += new System.EventHandler(this.customGLControl1_Load);
            this.customGLControl1.Click += new System.EventHandler(this.customGLControl1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 402);
            this.Controls.Add(this._sideTabs);
            this.Controls.Add(this._statusBar);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.customGLControl1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Mostacho Editor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this._statusBar.ResumeLayout(false);
            this._statusBar.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this._sideTabs.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomGLControl customGLControl1;
        private System.Windows.Forms.StatusStrip _statusBar;
        private System.Windows.Forms.ToolStripStatusLabel tileCoordinates;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView _tileSelector;
        private System.Windows.Forms.TabControl _sideTabs;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel layerNumber;
        private System.Windows.Forms.ListView _tileFileList;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel mode;
        private System.Windows.Forms.ToolStripMenuItem padToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;

    }
}

