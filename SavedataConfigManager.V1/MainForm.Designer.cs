namespace SavedataConfigManager
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            toolStripChooseGameFolderButton = new ToolStripButton();
            toolStripAddFileButton = new ToolStripButton();
            toolStripAddFolderButton = new ToolStripButton();
            toolStripDeleteButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripGameFolderPathTextBox = new ToolStripTextBox();
            entryDataGridView = new DataGridView();
            id = new DataGridViewTextBoxColumn();
            pathMode = new DataGridViewComboBoxColumn();
            fsType = new DataGridViewComboBoxColumn();
            path = new DataGridViewTextBoxColumn();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)entryDataGridView).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, closeToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(39, 21);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(208, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(208, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            saveAsToolStripMenuItem.Size = new Size(208, 22);
            saveAsToolStripMenuItem.Text = "Save As...";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(208, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.AllowDrop = true;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripChooseGameFolderButton, toolStripAddFileButton, toolStripAddFolderButton, toolStripDeleteButton, toolStripSeparator1, toolStripGameFolderPathTextBox });
            toolStrip1.Location = new Point(0, 25);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            toolStrip1.DragDrop += toolStrip1_DragDrop;
            toolStrip1.DragEnter += toolStrip1_DragEnter;
            // 
            // toolStripChooseGameFolderButton
            // 
            toolStripChooseGameFolderButton.Image = (Image)resources.GetObject("toolStripChooseGameFolderButton.Image");
            toolStripChooseGameFolderButton.ImageTransparentColor = Color.Magenta;
            toolStripChooseGameFolderButton.Name = "toolStripChooseGameFolderButton";
            toolStripChooseGameFolderButton.Size = new Size(151, 22);
            toolStripChooseGameFolderButton.Text = "Choose Game Folder";
            toolStripChooseGameFolderButton.Click += toolStripChooseGameFolderButton_Click;
            // 
            // toolStripAddFileButton
            // 
            toolStripAddFileButton.Image = (Image)resources.GetObject("toolStripAddFileButton.Image");
            toolStripAddFileButton.ImageTransparentColor = Color.Magenta;
            toolStripAddFileButton.Name = "toolStripAddFileButton";
            toolStripAddFileButton.Size = new Size(75, 22);
            toolStripAddFileButton.Text = "Add File";
            toolStripAddFileButton.Click += toolStripAddFileButton_Click;
            // 
            // toolStripAddFolderButton
            // 
            toolStripAddFolderButton.Image = (Image)resources.GetObject("toolStripAddFolderButton.Image");
            toolStripAddFolderButton.ImageTransparentColor = Color.Magenta;
            toolStripAddFolderButton.Name = "toolStripAddFolderButton";
            toolStripAddFolderButton.Size = new Size(93, 22);
            toolStripAddFolderButton.Text = "Add Folder";
            toolStripAddFolderButton.Click += toolStripAddFolderButton_Click;
            // 
            // toolStripDeleteButton
            // 
            toolStripDeleteButton.Image = (Image)resources.GetObject("toolStripDeleteButton.Image");
            toolStripDeleteButton.ImageTransparentColor = Color.Magenta;
            toolStripDeleteButton.Name = "toolStripDeleteButton";
            toolStripDeleteButton.Size = new Size(121, 22);
            toolStripDeleteButton.Text = "Delete Selection";
            toolStripDeleteButton.Click += toolStripDeleteButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripGameFolderPathTextBox
            // 
            toolStripGameFolderPathTextBox.Name = "toolStripGameFolderPathTextBox";
            toolStripGameFolderPathTextBox.ReadOnly = true;
            toolStripGameFolderPathTextBox.Size = new Size(325, 25);
            // 
            // entryDataGridView
            // 
            entryDataGridView.AllowDrop = true;
            entryDataGridView.AllowUserToOrderColumns = true;
            entryDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            entryDataGridView.Columns.AddRange(new DataGridViewColumn[] { id, pathMode, fsType, path });
            entryDataGridView.Dock = DockStyle.Fill;
            entryDataGridView.Location = new Point(0, 50);
            entryDataGridView.Name = "entryDataGridView";
            entryDataGridView.Size = new Size(800, 400);
            entryDataGridView.TabIndex = 3;
            entryDataGridView.DragDrop += entryDataGridView_DragDrop;
            entryDataGridView.DragEnter += entryDataGridView_DragEnter;
            // 
            // id
            // 
            id.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id.HeaderText = "Id";
            id.Name = "id";
            id.Width = 45;
            // 
            // pathMode
            // 
            pathMode.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pathMode.HeaderText = "PathMode";
            pathMode.Items.AddRange(new object[] { "Absolute", "Game", "Document", "Profile" });
            pathMode.Name = "pathMode";
            pathMode.Width = 74;
            // 
            // fsType
            // 
            fsType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            fsType.HeaderText = "FSType";
            fsType.Items.AddRange(new object[] { "File", "Folder" });
            fsType.Name = "fsType";
            fsType.Width = 55;
            // 
            // path
            // 
            path.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            path.HeaderText = "Path";
            path.Name = "path";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(entryDataGridView);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "SaveData Config Manager V1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)entryDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripAddFileButton;
        private ToolStripButton toolStripAddFolderButton;
        private ToolStripButton toolStripDeleteButton;
        private ToolStripButton toolStripChooseGameFolderButton;
        private ToolStripTextBox toolStripGameFolderPathTextBox;
        private ToolStripSeparator toolStripSeparator1;
        private DataGridView entryDataGridView;
        private DataGridViewTextBoxColumn id;
        private DataGridViewComboBoxColumn pathMode;
        private DataGridViewComboBoxColumn fsType;
        private DataGridViewTextBoxColumn path;
    }
}