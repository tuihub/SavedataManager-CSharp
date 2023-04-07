namespace SavedataManagerGui
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
            tableLayoutPanel1 = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            restoreButton = new Button();
            storeButton = new Button();
            label1 = new Label();
            label2 = new Label();
            gamePathTextBox = new TextBox();
            savedataFileTextBox = new TextBox();
            folderSelectButton = new Button();
            fileSelectButton = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 2);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(gamePathTextBox, 1, 0);
            tableLayoutPanel1.Controls.Add(savedataFileTextBox, 1, 1);
            tableLayoutPanel1.Controls.Add(folderSelectButton, 2, 0);
            tableLayoutPanel1.Controls.Add(fileSelectButton, 2, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 25);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(400, 185);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 3);
            flowLayoutPanel1.Controls.Add(restoreButton);
            flowLayoutPanel1.Controls.Add(storeButton);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new Point(3, 151);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(394, 31);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // restoreButton
            // 
            restoreButton.Location = new Point(304, 3);
            restoreButton.Margin = new Padding(3, 3, 15, 3);
            restoreButton.Name = "restoreButton";
            restoreButton.Size = new Size(75, 23);
            restoreButton.TabIndex = 0;
            restoreButton.Text = "Restore";
            restoreButton.UseVisualStyleBackColor = true;
            restoreButton.Click += restoreButton_Click;
            // 
            // storeButton
            // 
            storeButton.Location = new Point(211, 3);
            storeButton.Margin = new Padding(3, 3, 15, 3);
            storeButton.Name = "storeButton";
            storeButton.Size = new Size(75, 23);
            storeButton.TabIndex = 1;
            storeButton.Text = "Store";
            storeButton.UseVisualStyleBackColor = true;
            storeButton.Click += storeButton_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(3, 20);
            label1.Name = "label1";
            label1.Size = new Size(64, 34);
            label1.TabIndex = 1;
            label1.Text = "Game Path";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(3, 94);
            label2.Name = "label2";
            label2.Size = new Size(64, 34);
            label2.TabIndex = 2;
            label2.Text = "Savedata File";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // gamePathTextBox
            // 
            gamePathTextBox.AllowDrop = true;
            gamePathTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            gamePathTextBox.Location = new Point(73, 25);
            gamePathTextBox.Name = "gamePathTextBox";
            gamePathTextBox.ReadOnly = true;
            gamePathTextBox.Size = new Size(258, 23);
            gamePathTextBox.TabIndex = 3;
            gamePathTextBox.DragDrop += gamePathTextBox_DragDrop;
            gamePathTextBox.DragEnter += gamePathTextBox_DragEnter;
            // 
            // savedataFileTextBox
            // 
            savedataFileTextBox.AllowDrop = true;
            savedataFileTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            savedataFileTextBox.Location = new Point(73, 99);
            savedataFileTextBox.Name = "savedataFileTextBox";
            savedataFileTextBox.ReadOnly = true;
            savedataFileTextBox.Size = new Size(258, 23);
            savedataFileTextBox.TabIndex = 4;
            savedataFileTextBox.DragDrop += savedataFileTextBox_DragDrop;
            savedataFileTextBox.DragEnter += savedataFileTextBox_DragEnter;
            // 
            // folderSelectButton
            // 
            folderSelectButton.Anchor = AnchorStyles.Left;
            folderSelectButton.Location = new Point(337, 25);
            folderSelectButton.Name = "folderSelectButton";
            folderSelectButton.Size = new Size(30, 23);
            folderSelectButton.TabIndex = 5;
            folderSelectButton.Text = "...";
            folderSelectButton.UseVisualStyleBackColor = true;
            folderSelectButton.Click += folderSelectButton_Click;
            // 
            // fileSelectButton
            // 
            fileSelectButton.Anchor = AnchorStyles.Left;
            fileSelectButton.Location = new Point(337, 99);
            fileSelectButton.Name = "fileSelectButton";
            fileSelectButton.Size = new Size(30, 23);
            fileSelectButton.TabIndex = 6;
            fileSelectButton.Text = "...";
            fileSelectButton.UseVisualStyleBackColor = true;
            fileSelectButton.Click += fileSelectButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(400, 25);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(39, 21);
            fileToolStripMenuItem.Text = "File";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 210);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "Savedata Manager GUI";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button restoreButton;
        private Button storeButton;
        private Label label1;
        private Label label2;
        private TextBox gamePathTextBox;
        private TextBox savedataFileTextBox;
        private Button folderSelectButton;
        private Button fileSelectButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
    }
}