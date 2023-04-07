using log4net;
using SavedataManagerGui.Utils;
using System.Windows.Forms;

namespace SavedataManagerGui
{
    public partial class MainForm : Form
    {
        private readonly TuiHub.SavedataManagerLibrary.SavedataManager _manager = new(LogManager.GetLogger(typeof(TuiHub.SavedataManagerLibrary.SavedataManager)));

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
                gamePathTextBox.Text = folderPath;
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
            var gamePath = gamePathTextBox.Text;
            if (string.IsNullOrEmpty(gamePath))
            {
                MessageBox.Show("Must select game path first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var memoryStream = _manager.Store(gamePath);
                if (memoryStream == null)
                {
                    MessageBox.Show("Store failed", "Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            var gamePath = gamePathTextBox.Text;
            if (string.IsNullOrEmpty(gamePath))
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
                if (_manager.CheckFSLastWriteTimeNewer(savedataFilePath, gamePath) == true)
                {
                    var dialogResult = MessageBox.Show("Current App savedata is newer than the one to restore, overwrite?",
                                                       "Overwrite", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dialogResult != DialogResult.Yes)
                    {
                        return;
                    }
                }
                var result = _manager.Restore(savedataFilePath, gamePath, true);
                if (result == true)
                {
                    MessageBox.Show("Restore succeed", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Store failed", "Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Unknown", "Restore error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}