using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System;
using Azure.Storage.Blobs;  
using System.Threading.Tasks;


namespace Paragoniarz
{
    public partial class FindFileControl : UserControl
    {

        private int _userId;
        public FindFileControl(int userId)
        {
            InitializeComponent();
            _userId = userId;
            tableLayoutPanel1.Controls.Clear();
            WindowHelper.SetWindowRoundCorners(panel6,10);

        }

        // rysowanie gradientu na panelu
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

        
        private void button3_Click(object sender,EventArgs e)
        {
            // Pobierz dane z TextBoxów
            string nazwa = textBox8.Text;
            string opis = textBox7.Text; // Możesz użyć tego później, jeśli chcesz dodać opcję opisu
            string osoba = textBox1.Text; // Może być przydatne, jeśli chcesz filtrować po osobie
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
            string query = finderManager.CreateSearchQuery(_userId,nazwa,dataOd,dataDo);

            // Wysyłamy zapytanie do bazy danych
            DatabaseHelper dbConnection = new DatabaseHelper();
            DataTable result = dbConnection.GetDataFromQuery(query);

            if (result.Rows.Count > 0)
            {
               // Wyczyść istniejące dane w TableLayoutPanel przed dodaniem nowych
                tableLayoutPanel1.Controls.Clear();
                tableLayoutPanel1.RowCount = 1;  // Resetowanie liczby wierszy do 1
                tableLayoutPanel1.SuspendLayout();

                // Iterujemy przez wszystkie wiersze wyników
                foreach (DataRow row in result.Rows)
                {
                    

                    // Kolumna 0: Nazwa pliku
                    Label labelNazwa = new Label();
                    labelNazwa.Text = row["original_name"].ToString();
                    labelNazwa.TextAlign = ContentAlignment.MiddleCenter;
                    tableLayoutPanel1.Controls.Add(labelNazwa,0,tableLayoutPanel1.RowCount - 1);
                    labelNazwa.ForeColor = Color.White;


                    // Kolumna 1: Data dodania (timestamp)
                    Label labelData = new Label();
                    labelData.Text = row["timestamp"].ToString(); // Możesz dodać formatowanie daty, jeśli potrzeba
                    labelData.TextAlign = ContentAlignment.MiddleCenter;
                    tableLayoutPanel1.Controls.Add(labelData,1,tableLayoutPanel1.RowCount - 1);
                    labelData.ForeColor = Color.White;

                    // Kolumna 2: Opis (jeśli nie masz danych, ustaw pustą wartość)
                    Label labelOpis = new Label();
                    labelOpis.Text = "3 kolumna"; // Zakładamy, że brak opisu w bazie
                    labelOpis.TextAlign = ContentAlignment.MiddleCenter;
                    tableLayoutPanel1.Controls.Add(labelOpis,2,tableLayoutPanel1.RowCount - 1);
                    labelOpis.ForeColor = Color.White;


                    // Kolumna 3: Załączony plik (URL)
                    Label labelUrl = new Label();
                    labelUrl.Text = row["file_url"].ToString();
                    labelUrl.TextAlign = ContentAlignment.MiddleCenter;
                    tableLayoutPanel1.Controls.Add(labelUrl,3,tableLayoutPanel1.RowCount - 1);
                    labelUrl.ForeColor = Color.White;


                    // Kolumna 4: Rozmiar (zakładamy, że nie masz danych o rozmiarze w bazie)
                    Label labelRozmiar = new Label();
                    labelRozmiar.Text = "5 kolumna"; // Możesz uzupełnić to danymi o rozmiarze, jeśli masz je w bazie
                    labelRozmiar.TextAlign = ContentAlignment.MiddleCenter;
                    tableLayoutPanel1.Controls.Add(labelRozmiar,4,tableLayoutPanel1.RowCount - 1);
                    labelRozmiar.ForeColor = Color.White;



                    Button deleteButton = new Button();
                    deleteButton.BackgroundImage = Properties.Resources.icons8_trash_26; // Ustawienie obrazka
                    deleteButton.BackgroundImageLayout = ImageLayout.Center; // Rozciągnięcie obrazka
                    deleteButton.FlatStyle = FlatStyle.Flat; // Usunięcie ramki
                    deleteButton.Size = new Size(31,33); // Dopasuj rozmiar przycisku
                    deleteButton.Cursor = Cursors.Hand;  // Zmieniamy kursor na rękę
                    deleteButton.Tag = row["original_name"]; // Przypisanie Tag z nazwą pliku (będzie użyteczne przy usuwaniu)
                    deleteButton.Click += button2_Click; // Podpięcie zdarzenia kliknięcia
                    tableLayoutPanel1.Controls.Add(deleteButton,5,tableLayoutPanel1.RowCount - 1);

                  


                    // Dodajemy kolejny wiersz
                    tableLayoutPanel1.RowCount++;

                }

                // Po zakończeniu dodawania wszystkich kontrolek wznawiamy układ
                tableLayoutPanel1.ResumeLayout();

            }
            else
            {
                MessageBox.Show("Brak wyników dla podanych kryteriów.");
            }
           

        }

        

        private void button1_Click(object sender,EventArgs e)
        {
            // Wyczyść istniejące dane w TableLayoutPanel przed dodaniem nowych
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 1;  // Resetowanie liczby wierszy do 1
        }



        public async Task DeleteFileFromBlobStorage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Nie znaleziono pliku do usunięcia.");
                return;
            }

            // Tworzymy obiekt BlobContainerClient
            BlobContainerClient containerClient = DatabaseConnection.Instance.CreateBlobStorageConnection();

            try
            {
                // Uzyskujemy obiekt BlobClient dla pliku, który chcemy usunąć
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                // Usuwamy plik
                await blobClient.DeleteIfExistsAsync();

                // Powiadamiamy użytkownika o sukcesie
                MessageBox.Show($"Plik {fileName} został pomyślnie usunięty.");
            }
            catch (Exception ex)
            {
                // Obsługuje wyjątek w przypadku błędu
                MessageBox.Show($"Błąd podczas usuwania pliku z Azure: {ex.Message}");
            }
        }

        //public async Task DeleteFileFromBlobStorage(string fileName)
        //{
        //    if (string.IsNullOrEmpty(fileName))
        //    {
        //        MessageBox.Show("Nie znaleziono pliku do usunięcia.");
        //        return;
        //    }

        //    var connectionString = DatabaseConnection.Instance.CreateBlobStorageConnection();
        //    Console.WriteLine(connectionString);
        //    string containerName = "documents";



        //    try
        //    {
        //        BlobContainerClient containerClient = new BlobContainerClient(connectionString,containerName);
        //        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        //        await blobClient.DeleteIfExistsAsync();
        //        MessageBox.Show($"Plik {fileName} został pomyślnie usunięty.");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Błąd podczas usuwania pliku z Azure: {ex.Message}");
        //    }
        //}






        private async void button2_Click(object sender,EventArgs e)
        {
            // Sprawdzamy, czy sender to przycisk
            Button deleteButton = sender as Button;

            if (deleteButton != null)
            {
                string fileName = deleteButton.Tag.ToString();  // Pobieramy nazwę pliku z Tag

                // Wywołanie funkcji usuwania pliku
                await DeleteFileFromBlobStorage(fileName);
            }
        }
    }
}
