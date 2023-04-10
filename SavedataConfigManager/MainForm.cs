using Models;
using SavedataConfigManager.Utils;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace SavedataConfigManager
{
    public partial class MainForm : Form
    {
        private OpenFileDialog _openJsonFileDialog, _openAnyFileDialog;
        private SaveFileDialog _saveFileDialog;
        private FolderBrowserDialog _folderBrowserDialog;

        public MainForm()
        {
            _openJsonFileDialog = new OpenFileDialog();
            _openJsonFileDialog.Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*";
            _openAnyFileDialog = new OpenFileDialog();
            _openAnyFileDialog.Filter = "All Files (*.*)|*.*";
            _saveFileDialog = new SaveFileDialog();
            _saveFileDialog.Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*";
            _saveFileDialog.DefaultExt = "json";
            _saveFileDialog.AddExtension = true;
            _folderBrowserDialog = new FolderBrowserDialog();

            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialogResult = _openJsonFileDialog.ShowDialog();
            var filePath = _openJsonFileDialog.FileName;
            if (dialogResult == DialogResult.OK && String.IsNullOrEmpty(filePath) == false)
            {
                Global.CurrentOpenedConfigFilePath = filePath;
                // change title - append current opened file path
                this.Text += " - " + filePath;
                // update toolStripGameFolderTextBox.Text
                toolStripGameFolderPathTextBox.Text = Path.GetDirectoryName(filePath);

                string configStr = File.ReadAllText(filePath, Global.UTF8WithoutBom);
                var config = JsonSerializer.Deserialize<Config>(configStr, Global.JsonSerializerOptions);
                entryDataGridView.Rows.Clear();
                foreach (var entry in config.Entries)
                {
                    entryDataGridView.AddItem(entry.Id, entry.PathMode, entry.GetFSType(), entry.Path);
                }
            }
        }

        private void toolStripAddFileButton_Click(object sender, EventArgs e)
        {
            var gameFolderPath = toolStripGameFolderPathTextBox.Text;
            if (string.IsNullOrEmpty(gameFolderPath))
            {
                MessageBox.Show("Must select game path first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var dialogResult = _openAnyFileDialog.ShowDialog();
            var filePath = _openAnyFileDialog.FileName;
            if (dialogResult == DialogResult.OK && String.IsNullOrEmpty(filePath) == false)
            {
                entryDataGridView.AddFile(gameFolderPath, filePath);
            }
        }

        private void toolStripChooseGameFolderButton_Click(object sender, EventArgs e)
        {
            var dialogResult = _folderBrowserDialog.ShowDialog();
            var folderPath = _folderBrowserDialog.SelectedPath;
            if (dialogResult == DialogResult.OK && String.IsNullOrEmpty(folderPath) == false)
            {
                toolStripGameFolderPathTextBox.Text = folderPath;
            }
        }

        private void toolStripAddFolderButton_Click(object sender, EventArgs e)
        {
            var gameFolderPath = toolStripGameFolderPathTextBox.Text;
            if (string.IsNullOrEmpty(gameFolderPath))
            {
                MessageBox.Show("Must select game path first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var dialogResult = _folderBrowserDialog.ShowDialog();
            var folderPath = _folderBrowserDialog.SelectedPath;
            if (dialogResult == DialogResult.OK && String.IsNullOrEmpty(folderPath) == false)
            {
                entryDataGridView.AddFolder(gameFolderPath, folderPath);
            }
        }

        private void toolStripDeleteButton_Click(object sender, EventArgs e)
        {
            if (entryDataGridView.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in entryDataGridView.SelectedRows)
                {
                    if (row.IsNewRow == false)
                        entryDataGridView.Rows.RemoveAt(row.Index);
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // change title - restore
            if (Global.CurrentOpenedConfigFilePath != null)
                this.Text = this.Text.Remove(this.Text.Length - (Global.CurrentOpenedConfigFilePath.Length + 3));
            Global.CurrentOpenedConfigFilePath = null;
            entryDataGridView.Rows.Clear();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Global.CurrentOpenedConfigFilePath == null)
            {
                MessageBox.Show("Must open config file first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var config = ConfigModelHelper.GenerateConfigFromEntryDataGridView(entryDataGridView);
            var configStr = JsonSerializer.Serialize(config, Global.JsonSerializerOptions);
            File.WriteAllText(Global.CurrentOpenedConfigFilePath, configStr, Global.UTF8WithoutBom);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // add default name
            _saveFileDialog.FileName = Global.SavedataConfigFileName;
            // always set initial dir to game folder path
            _saveFileDialog.InitialDirectory = toolStripGameFolderPathTextBox.Text;
            var dialogResult = _saveFileDialog.ShowDialog();
            var filePath = _saveFileDialog.FileName;
            if (dialogResult == DialogResult.OK && String.IsNullOrEmpty(filePath) == false)
            {
                var config = ConfigModelHelper.GenerateConfigFromEntryDataGridView(entryDataGridView);
                var configStr = JsonSerializer.Serialize(config, Global.JsonSerializerOptions);
                File.WriteAllText(filePath, configStr, Global.UTF8WithoutBom);
            }
        }

        private void entryDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            var gameFolderPath = toolStripGameFolderPathTextBox.Text;
            if (string.IsNullOrEmpty(gameFolderPath))
            {
                MessageBox.Show("Must select game path first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                foreach (var file in files)
                {
                    // is folder
                    if (Directory.Exists(file))
                    {
                        entryDataGridView.AddFolder(gameFolderPath, file);
                    }
                    // is file
                    else
                    {
                        entryDataGridView.AddFile(gameFolderPath, file);
                    }
                }
            }
        }

        // from https://www.codeproject.com/Tips/5257022/Allowing-Explorer-to-Drop-Files-on-a-WinForms-App
        private void entryDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void toolStrip1_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Count() == 1 && Directory.Exists(files[0]))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void toolStrip1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Count() == 1 && Directory.Exists(files[0]))
            {
                toolStripGameFolderPathTextBox.Text = files[0];
            }
        }
    }
}