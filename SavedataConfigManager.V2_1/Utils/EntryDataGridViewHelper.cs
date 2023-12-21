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
        public static void AddItem(this DataGridView dataGridView, long id, FilePatternType patternType, string pattern, bool clearDir, BaseDirMode baseDirMode, string baseDirPath)
        {
            dataGridView.Rows.Add(id.ToString(), patternType.ToString(), pattern, clearDir, baseDirMode.ToString(), baseDirPath);
        }
        public static void AddItem(this DataGridView dataGridView, long id, string patternType, string pattern, bool clearDir, string baseDirMode, string baseDirPath)
        {
            dataGridView.Rows.Add(id.ToString(), patternType, pattern, clearDir, baseDirMode, baseDirPath);
        }
        public static void AddFolder(this DataGridView entryDataGridView, string gamePath, string folderPath)
        {
            long maxId = 0;
            var rows = entryDataGridView.Rows.Cast<DataGridViewRow>();
            if (entryDataGridView.Rows.Count > 1)
                maxId = rows.Where(x => x.IsNewRow == false)
                            .Max(x => long.Parse(x.Cells[0].Value.ToString()));
            PathHelper.Convert(gamePath, ref folderPath, out BaseDirMode baseDirMode);
            var row = rows.FirstOrDefault(x => x.IsNewRow == false &&
                                               x.Cells[4].Value.ToString() == baseDirMode.ToString() && 
                                               x.Cells[5].Value.ToString() == folderPath);
            var id = row == null ? maxId + 1 : long.Parse(row.Cells[0].Value.ToString());
            entryDataGridView.AddItem(id, FilePatternType.Include, "*", false, baseDirMode, folderPath);
        }
        public static void AddFile(this DataGridView entryDataGridView, string gamePath, string filePath)
        {
            long maxId = 0;
            var rows = entryDataGridView.Rows.Cast<DataGridViewRow>();
            if (entryDataGridView.Rows.Count > 1)
                maxId = rows.Where(x => x.IsNewRow == false)
                            .Max(x => long.Parse(x.Cells[0].Value.ToString()));
            PathHelper.Convert(gamePath, ref filePath, out BaseDirMode baseDirMode);
            var folderPath = Path.GetDirectoryName(filePath) ?? ".";
            var row = rows.FirstOrDefault(x => x.IsNewRow == false &&
                                               x.Cells[4].Value.ToString() == baseDirMode.ToString() &&
                                               x.Cells[5].Value.ToString() == folderPath);
            var id = row == null ? maxId + 1 : long.Parse(row.Cells[0].Value.ToString());
            entryDataGridView.AddItem(id, FilePatternType.Include, Path.GetFileName(filePath), false, baseDirMode, folderPath);
        }
    }
}
