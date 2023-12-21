using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataConfigManager.Utils
{
    internal class ConfigModelHelper
    {
        public static Config GenerateConfigFromEntryDataGridView(DataGridView entryDataGridView)
        {
            var config = new Config();
            config.Platform = Platform.Windows;
            config.Entries ??= new List<Entry>();
            var groupedRows = entryDataGridView.Rows.Cast<DataGridViewRow>()
                .Where(x => x.IsNewRow == false)
                .GroupBy(x => long.Parse(x.Cells[0].Value.ToString()))
                .OrderBy(g => g.Key);
            foreach (var group in groupedRows)
            {
                var entry = new Entry();
                entry.Id = group.Key;
                entry.BaseDirMode = (BaseDirMode)Enum.Parse(typeof(BaseDirMode), group.First().Cells[4].Value.ToString());
                entry.BaseDir = group.First().Cells[5].Value.ToString();
                entry.FilePatterns ??= new List<FilePattern>();
                foreach (var row in group)
                {
                    var patternType = (FilePatternType)Enum.Parse(typeof(FilePatternType), row.Cells[1].Value.ToString());
                    var pattern = row.Cells[2].Value.ToString();
                    entry.FilePatterns.Add(new FilePattern { Type = patternType, Pattern = pattern });
                }
                entry.ClearBaseDirBeforeRestore = bool.Parse(group.First().Cells[3].Value.ToString());
                config.Entries.Add(entry);
            }
            return config;
        }
    }
}
