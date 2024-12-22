using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paragoniarz
{
    internal class BuildingTable
    {
        private TableLayoutPanel _tableLayoutPanel;

        public BuildingTable(TableLayoutPanel tableLayoutPanel)
        {
            _tableLayoutPanel = tableLayoutPanel;
        }

        public void PopulateTableWithData(DataTable result)
        {
            if (result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Label label = new Label();

                        if (i == 0)
                            label.Text = row["original_name"].ToString();
                        else if (i == 1)
                            label.Text = row["file_url"].ToString();
                        else if (i == 2)
                            label.Text = row["timestamp"].ToString();

                        _tableLayoutPanel.Controls.Add(label,i,_tableLayoutPanel.RowCount);
                    }
                    _tableLayoutPanel.RowCount++;
                }
            }
        }

    }
}
