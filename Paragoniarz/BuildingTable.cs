using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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

        public void RefreshTable()
        {
            //TODO: Wpisac zapytanie poprawne
            // Pobierz dane z bazy (wstaw swoje zapytanie)
            string query = "SELECT * FROM YourTable";  // Przykładowe zapytanie SQL
            DatabaseHelper dbConnection = new DatabaseHelper();
            DataTable result = dbConnection.GetDataFromQuery(query);

            // Ponownie budujemy tabelę z nowymi danymi
            PopulateTableWithData(result);
        }

        public void PopulateTableWithData(DataTable result)
        {
            if (result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    // Tworzymy nowy wiersz
                    _tableLayoutPanel.RowCount++;

                    // Kolumny z danych w bazie (dopasowane do kolejności)
                    string nazwa = row["original_name"].ToString();
                    string data = row["timestamp"].ToString();
                    string zalaczonyPlik = row["file_url"].ToString();
                    string rozmiar = row["file_size"].ToString(); // Załóżmy, że masz kolumnę "file_size" w bazie

                }
            }
        }

    }
}
