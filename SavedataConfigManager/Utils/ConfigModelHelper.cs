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
            foreach (DataGridViewRow row in entryDataGridView.Rows)
            {
                if (row.IsNewRow == true) continue;
                var id = long.Parse(row.Cells[0].Value.ToString());
                var pathMode = (PathMode)Enum.Parse(typeof(PathMode), row.Cells[1].Value.ToString());
                var path = row.Cells[3].Value.ToString();
                //path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                config.Entries.Add(new Entry { Id = id, PathMode = pathMode, Path = path });
            }
            return config;
        }
    }
}
