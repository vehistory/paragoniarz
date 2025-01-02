using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;
using Azure.Storage.Blobs;
using MySqlX.XDevAPI;

namespace Paragoniarz
{
    public partial class FindFileControl : UserControl
    {
        private int _userId;

        private static readonly HttpClient client = new HttpClient();

        public FindFileControl(int userId)
        {
            InitializeComponent();
            _userId = userId;
            tableLayoutPanel1.Controls.Clear();
            WindowHelper.SetWindowRoundCorners(panel6, 10);
        }

        // rysowanie gradientu na panelu
        private void gradientPanel(object sender, PaintEventArgs e, Panel panel)
        {
            // Definicja kolorów dla gradientu
            Color startColor = Color.FromArgb(0, 64, 95); // Kolor początkowy (np. czerwony)
            Color endColor = Color.FromArgb(64, 64, 64); // Kolor końcowy (np. niebieski)
            // Tworzymy pędzel z gradientem
            using (
                LinearGradientBrush brush = new LinearGradientBrush(
                    panel.ClientRectangle, // Określamy obszar panelu
                    startColor, // Kolor początkowy
                    endColor, // Kolor końcowy
                    45F
                )
            ) // Kąt gradientu (w tym przypadku 45 stopni)
            {
                // Rysujemy tło panelu z użyciem gradientu
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            gradientPanel(sender, e, panel6);
        }

        private void tablePanel_Scroll(object sender, ScrollEventArgs e)
        {
            tablePanel.AutoScroll = true;
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tablePanel.Controls.Add(tableLayoutPanel1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Pobierz dane z TextBoxów
            string nazwa = textBox8.Text;
            string opis = textBox7.Text; // Możesz użyć tego później, jeśli chcesz dodać opcję opisu
            string osoba = textBox1.Text; // Może być przydatne, jeśli chcesz filtrować po osobie
            DateTime? dataOd = null;
            DateTime? dataDo = null;

            // Sprawdzamy, czy daty są wpisane (można użyć DateTimePickerów zamiast TextBoxów)
            if (DateTime.TryParse(dateTimePicker1.Text, out DateTime parsedDataOd))
            {
                dataOd = parsedDataOd;
            }

            if (DateTime.TryParse(dateTimePicker2.Text, out DateTime parsedDataDo))
            {
                dataDo = parsedDataDo;
            }

            // Tworzymy instancję klasy odpowiedzialnej za zapytanie
            FinderManager finderManager = new FinderManager();
            string query = finderManager.CreateSearchQuery(_userId, nazwa, dataOd, dataDo);

            // Wysyłamy zapytanie do bazy danych
            DatabaseHelper dbConnection = new DatabaseHelper();
            DataTable result = dbConnection.GetDataFromQuery(query);

            if (result.Rows.Count > 0)
            {
                // Wyczyść istniejące dane w TableLayoutPanel przed dodaniem nowych
                tableLayoutPanel1.Controls.Clear();
                tableLayoutPanel1.RowCount = 0; // Resetowanie liczby wierszy do 0
                tableLayoutPanel1.SuspendLayout();

                // Iterujemy przez wszystkie wiersze wyników
                foreach (DataRow row in result.Rows)
                {
                    tableLayoutPanel1.RowCount++;
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

                    string url = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(row["file_url"].ToString()));

                    // Kolumna 0: Nazwa pliku
                    LinkLabel labelNazwa = new LinkLabel();
                    labelNazwa.Text = row["original_name"].ToString();
                    labelNazwa.LinkClicked += new LinkLabelLinkClickedEventHandler(Download_fileAsync);
                    labelNazwa.AccessibleName = url;
                    labelNazwa.TextAlign = ContentAlignment.MiddleCenter;
                    labelNazwa.LinkColor = Color.FromArgb(0, 192, 192);
                    labelNazwa.Dock = DockStyle.Fill;
                    labelNazwa.AutoSize = false;
                    labelNazwa.AutoEllipsis = true;
                    tableLayoutPanel1.Controls.Add(labelNazwa, 0, tableLayoutPanel1.RowCount - 1);

                    // Kolumna 1: Data dodania (timestamp)
                    Label labelData = new Label();
                    labelData.Text = row["timestamp"].ToString();
                    labelData.TextAlign = ContentAlignment.MiddleCenter;
                    labelData.ForeColor = Color.FromArgb(0, 192, 192);
                    labelData.Dock = DockStyle.Fill;
                    labelData.AutoSize = false;
                    tableLayoutPanel1.Controls.Add(labelData, 1, tableLayoutPanel1.RowCount - 1);

                    // Kolumna 2: Opis
                    Label labelOpis = new Label();
                    labelOpis.Text = "3 kolumna";
                    labelOpis.TextAlign = ContentAlignment.MiddleCenter;
                    labelOpis.ForeColor = Color.FromArgb(0, 192, 192);
                    labelOpis.Dock = DockStyle.Fill;
                    labelOpis.AutoSize = false;
                    tableLayoutPanel1.Controls.Add(labelOpis, 2, tableLayoutPanel1.RowCount - 1);

                    // Kolumna 3: Załączony plik (URL)
                    LinkLabel labelUrl = new LinkLabel();
                    labelUrl.AccessibleName = url;
                    labelUrl.Text = "Wyświetl";
                    labelUrl.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
                    labelUrl.TextAlign = ContentAlignment.MiddleCenter;
                    labelUrl.LinkColor = Color.FromArgb(0, 192, 192);
                    labelUrl.Dock = DockStyle.Fill;
                    labelUrl.AutoSize = false;
                    tableLayoutPanel1.Controls.Add(labelUrl, 3, tableLayoutPanel1.RowCount - 1);

                    // Kolumna 4: Rozmiar
                    Label labelRozmiar = new Label();
                    labelRozmiar.Text = "5 kolumna";
                    labelRozmiar.TextAlign = ContentAlignment.MiddleCenter;
                    labelRozmiar.ForeColor = Color.FromArgb(0, 192, 192);
                    labelRozmiar.Dock = DockStyle.Fill;
                    labelRozmiar.AutoSize = false;
                    labelRozmiar.AutoEllipsis = true;
                    tableLayoutPanel1.Controls.Add(labelRozmiar, 4, tableLayoutPanel1.RowCount - 1);

                    // Przycisk usuwania pozostaje bez zmian
                    Button deleteButton = new Button();
                    deleteButton.BackgroundImage = Properties.Resources.icons8_trash_26;
                    deleteButton.BackgroundImageLayout = ImageLayout.Center;
                    deleteButton.FlatStyle = FlatStyle.Flat;
                    deleteButton.Size = new Size(31, 33);
                    deleteButton.Cursor = Cursors.Hand;
                    deleteButton.Tag = row["original_name"];
                    deleteButton.Click += button2_Click;
                    labelRozmiar.Dock = DockStyle.Fill;
                    labelRozmiar.AutoSize = false;
                    tableLayoutPanel1.Controls.Add(deleteButton, 5, tableLayoutPanel1.RowCount - 1);
                }

                // Po zakończeniu dodawania wszystkich kontrolek wznawiamy układ
                tableLayoutPanel1.ResumeLayout();
            }
            else
            {
                MessageBox.Show("Brak wyników dla podanych kryteriów.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Wyczyść istniejące dane w TableLayoutPanel przed dodaniem nowych
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 1; // Resetowanie liczby wierszy do 1
        }

        public async Task DeleteFileFromBlobStorage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Nie znaleziono pliku do usunięcia.");
                return;
            }

            // Tworzymy obiekt BlobContainerClient
            BlobContainerClient containerClient =
                DatabaseConnection.Instance.CreateBlobStorageConnection();

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

        private async void button2_Click(object sender, EventArgs e)
        {
            // Sprawdzamy, czy sender to przycisk
            Button deleteButton = sender as Button;

            if (deleteButton != null)
            {
                string fileName = deleteButton.Tag.ToString(); // Pobieramy nazwę pliku z Tag

                // Wywołanie funkcji usuwania pliku
                await DeleteFileFromBlobStorage(fileName);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel clickedLabel = sender as LinkLabel;
            if (clickedLabel != null)
            {
                string url = clickedLabel.AccessibleName;
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie udało się otworzyć linku: {ex.Message}");
                }
            }
        }

        private async void Download_fileAsync(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel clickedLabel = sender as LinkLabel;

            string expandedPath = Environment.ExpandEnvironmentVariables(@"%userprofile%\Downloads");
            string savePath = Path.Combine(expandedPath, clickedLabel.Text);
            string fileUrl = clickedLabel.AccessibleName;
            try
            {
                byte[] fileBytes = await client.GetByteArrayAsync(fileUrl);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                File.WriteAllBytes(savePath, fileBytes);

                MessageBox.Show($"Plik został pobrany do: {savePath}");

                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(savePath)
                    {
                        UseShellExecute = true
                    });
                }
                catch (Exception openEx)
                {
                    MessageBox.Show($"Plik został pobrany, ale nie udało się go otworzyć: {openEx.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}");
            }
        }
    }
}
