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

       

        private void AddLabelToTable(int columnIndex,string text)
        {
            Label label = new Label();
            label.Text = string.IsNullOrEmpty(text) ? "Brak danych" : text;  // Ustawiamy tekst, jeśli brak danych, wyświetlamy "Brak danych"
            label.ForeColor = Color.White;  // Ustawiamy kolor tekstu (możesz dostosować)
            label.TextAlign = ContentAlignment.MiddleCenter;  // Wyrównanie tekstu
            label.BackColor = Color.Transparent;  // Tło białe

            // Dodajemy etykietę do odpowiedniej kolumny
            _tableLayoutPanel.Controls.Add(label,columnIndex,_tableLayoutPanel.RowCount - 1);
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

                    // Dodajemy dane do odpowiednich komórek w tabeli
                    AddLabelToTable(0,nazwa);           // Kolumna 0: Nazwa
                    AddLabelToTable(1,data);            // Kolumna 1: Data
                    AddLabelToTable(2,"xcvbxcvb");              // Kolumna 2: Opis (na razie pusta)
                    AddLabelToTable(3,zalaczonyPlik);   // Kolumna 3: Załączony plik (URL)
                    AddLabelToTable(4,rozmiar);         // Kolumna 4: Rozmiar
                    AddLabelToTable(5,"xcvb");              // Kolumna 5: Pusta kolumna
                }
            }
        }

    }
}
