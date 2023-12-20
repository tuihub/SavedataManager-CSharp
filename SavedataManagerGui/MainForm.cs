using SavedataManagerGui.Utils;
using System.Windows.Forms;

namespace SavedataManagerGui
{
    public partial class MainForm : Form
    {
        private readonly TuiHub.SavedataManagerLibrary.SavedataManager _manager = new();

        private OpenFileDialog _openZipFileDialog;
        private SaveFileDialog _saveZipFileDialog;
        private FolderBrowserDialog _folderBrowserDialog;
        public MainForm()
        {
            _openZipFileDialog = new OpenFileDialog();
            _openZipFileDialog.Filter = "Zip Files (*.zip)|*.zip|All Files (*.*)|*.*";
            _saveZipFileDialog = new SaveFileDialog();
            _saveZipFileDialog.Filter = "Zip Files (*.zip)|*.zip";
            _saveZipFileDialog.DefaultExt = "zip";
            _saveZipFileDialog.AddExtension = true;
            _folderBrowserDialog = new FolderBrowserDialog();
            InitializeComponent();
        }

        private void folderSelectButton_Click(object sender, EventArgs e)
        {
            var dialogResult = _folderBrowserDialog.ShowDialog();
            var folderPath = _folderBrowserDialog.SelectedPath;
            if (dialogResult == DialogResult.OK && String.IsNullOrEmpty(folderPath) == false)
            {
                gameFolderPathTextBox.Text = folderPath;
            }
        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            var dialogResult = _openZipFileDialog.ShowDialog();
            var filePath = _openZipFileDialog.FileName;
            if (dialogResult == DialogResult.OK && String.IsNullOrEmpty(filePath) == false)
            {
                savedataFileTextBox.Text = filePath;
            }
        }

        private void storeButton_Click(object sender, EventArgs e)
        {
            var gameFolderPath = gameFolderPathTextBox.Text;
            using var memoryStream = new MemoryStream();
            if (string.IsNullOrEmpty(gameFolderPath))
            {
                MessageBox.Show("Must select game path first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _manager.Store(gameFolderPath, memoryStream);
                if (string.IsNullOrEmpty(_saveZipFileDialog.FileName)) _saveZipFileDialog.FileName = "dummy.zip";
                var dialogResult = _saveZipFileDialog.ShowDialog();
                var filePath = _saveZipFileDialog.FileName;
                if (dialogResult == DialogResult.OK && String.IsNullOrEmpty(filePath) == false)
                {
                    FileHelper.Write(memoryStream, filePath);
                    MessageBox.Show("Store succeed", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Unknown", "Store error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                memoryStream.Close();
                memoryStream.Dispose();
            }
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            var gameFolderPath = gameFolderPathTextBox.Text;
            if (string.IsNullOrEmpty(gameFolderPath))
            {
                MessageBox.Show("Must select game path first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var savedataFilePath = savedataFileTextBox.Text;
            if (string.IsNullOrEmpty(savedataFilePath))
            {
                MessageBox.Show("Must select savedata file path first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using var fs = File.OpenRead(savedataFilePath);
                if (_manager.CheckFSLastWriteTimeNewer(fs, gameFolderPath) == true)
                {
                    var dialogResult = MessageBox.Show("Current App savedata is newer than the one to restore, overwrite?",
                                                       "Overwrite", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dialogResult != DialogResult.Yes)
                    {
                        return;
                    }
                }
                fs.Position = 0;
                _manager.Restore(fs, gameFolderPath, true);
                MessageBox.Show("Restore succeed", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Unknown", "Restore error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gameFolderPathTextBox_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Count() == 1 && Directory.Exists(files[0]))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void gameFolderPathTextBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Count() == 1 && Directory.Exists(files[0]))
            {
                gameFolderPathTextBox.Text = files[0];
            }
        }

        private void savedataFileTextBox_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Count() == 1 && Directory.Exists(files[0]) == false)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void savedataFileTextBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Count() == 1 && Directory.Exists(files[0]) == false)
            {
                savedataFileTextBox.Text = files[0];
            }
        }
    }
}