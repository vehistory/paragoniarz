using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data;
using System;

namespace Paragoniarz
{
    public partial class FindFileControl : UserControl
    {
        public FindFileControl()
        {
            InitializeComponent();

            
            WindowHelper.SetWindowRoundCorners(panel6,10);

        }


        private void gradientPanel(object sender,PaintEventArgs e, Panel panel)
        {
            // Definicja kolorów dla gradientu
            Color startColor = Color.FromArgb(0,64,95); // Kolor początkowy (np. czerwony)
            Color endColor = Color.FromArgb(64,64,64); // Kolor końcowy (np. niebieski)
            // Tworzymy pędzel z gradientem
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,  // Określamy obszar panelu
                startColor,              // Kolor początkowy
                endColor,                // Kolor końcowy
                45F))                    // Kąt gradientu (w tym przypadku 45 stopni)
            {
                // Rysujemy tło panelu z użyciem gradientu
                e.Graphics.FillRectangle(brush,panel.ClientRectangle);
            }
        }

       

        private void panel6_Paint(object sender,PaintEventArgs e)
        {
            gradientPanel(sender,e,panel6);
        }

        private void tablePanel_Scroll(object sender,ScrollEventArgs e)
        {
            
                tablePanel.AutoScroll = true;
            tableLayoutPanel1.Dock = DockStyle.Fill;
           tablePanel.Controls.Add(tableLayoutPanel1);

        }

        private void button3_Click(object sender,System.EventArgs e)
        {
            // Pobierz dane z TextBoxów
            string nazwa = textBox8.Text;
            string opis = textBox7.Text;
            string osoba = textBox1.Text;
            DateTime? dataOd = null;
            DateTime? dataDo = null;

            // Sprawdzamy, czy daty są wpisane (można użyć DateTimePickerów zamiast TextBoxów)
            if (DateTime.TryParse(dateTimePicker1.Text,out DateTime parsedDataOd))
            {
                dataOd = parsedDataOd;
            }

            if (DateTime.TryParse(dateTimePicker2.Text,out DateTime parsedDataDo))
            {
                dataDo = parsedDataDo;
            }

            // Tworzymy instancję klasy odpowiedzialnej za zapytanie
            FinderManager finderManager = new FinderManager();
            string query = finderManager.CreateSearchQuery(nazwa,opis,osoba,dataOd,dataDo);

            // Wysyłamy zapytanie do bazy danych
            DatabaseHelper dbConnection = new DatabaseHelper();
            DataTable result = dbConnection.GetDataFromQuery(query);

            // Tutaj możesz wyświetlić dane (np. w tabeli, na liście, itp.)
            // np. _tableManager.PopulateTableWithData(result); jeśli używasz TableManagera
        }
    }
}
