using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataConfigManager.Utils
{
    internal static class EntryDataGridViewHelper
    {
        public static void AddItem(this DataGridView dataGridView, long id, PathMode pathMode, EntryFSType fsType, string path)
        {
            dataGridView.Rows.Add(id.ToString(), pathMode.ToString(), fsType.ToString(), path);
        }
        public static void AddItem(this DataGridView dataGridView, long id, string pathMode, string fsType, string path)
        {
            dataGridView.Rows.Add(id.ToString(), pathMode, fsType, path);
        }
        public static void AddFolder(this DataGridView entryDataGridView, string gamePath, string folderPath)
        {
            long maxId = 0;
            if (entryDataGridView.Rows.Count > 1)
                maxId = entryDataGridView.Rows.Cast<DataGridViewRow>()
                    .Where(x => x.IsNewRow == false)
                    .Max(x => long.Parse(x.Cells[0].Value.ToString()));
            PathHelper.Convert(gamePath, ref folderPath, out PathMode pathMode);
            folderPath += System.IO.Path.DirectorySeparatorChar;
            entryDataGridView.AddItem(maxId + 1, pathMode, EntryFSType.Folder, folderPath);
        }
        public static void AddFile(this DataGridView entryDataGridView, string gamePath, string filePath)
        {
            long maxId = 0;
            if (entryDataGridView.Rows.Count > 1)
                maxId = entryDataGridView.Rows.Cast<DataGridViewRow>()
                    .Where(x => x.IsNewRow == false)
                    .Max(x => long.Parse(x.Cells[0].Value.ToString()));
            PathHelper.Convert(gamePath, ref filePath, out PathMode pathMode);
            entryDataGridView.AddItem(maxId + 1, pathMode, EntryFSType.File, filePath);
        }
    }
}
