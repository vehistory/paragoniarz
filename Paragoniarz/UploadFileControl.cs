using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Azure.Storage.Blobs;
using static Paragoniarz.DatabaseHelper;

namespace Paragoniarz
{
    public partial class UploadFileControl : UserControl
    {
        private string droppedFilePath; // Pole do przechowywania ścieżki pliku

        public UploadFileControl()
        {
            InitializeComponent();
            pictureBox2.AllowDrop = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearImageAndResetLabels();
        }

        private void ClearImageAndResetLabels()
        {
            if (pictureBox2.Image != null)
            {
                using (var image = pictureBox2.Image)
                {
                    pictureBox2.Image = null;
                }
            }

            ResetLabels();
        }

        private void ResetLabels()
        {
            FNlabel.Text = "Nazwa pliku: ";
            FSlabel.Text = "Rozmiar pliku: ";
            CDlabel.Text = "Data utworzenia: ";
            UDlabel.Text = "Data wgrania: ";
        }

        private void pictureBox2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    droppedFilePath = files[0]; // Zapisz ścieżkę pliku
                    FileInfo fileInfo = new FileInfo(droppedFilePath);

                    long maxSizeInBytes = 2 * 1024 * 1024; // 2 MB
                    if (fileInfo.Length > maxSizeInBytes)
                    {
                        MessageBox.Show("Plik jest zbyt duży. Maksymalny rozmiar to 2 MB.");
                        droppedFilePath = null; // Wyzeruj ścieżkę, jeśli plik jest zbyt duży
                        return;
                    }

                    string extension = Path.GetExtension(droppedFilePath).ToLower();

                    // Sprawdź rozszerzenie pliku
                    if (
                        extension == ".jpg"
                        || extension == ".jpeg"
                        || extension == ".png"
                        || extension == ".gif"
                        || extension == ".bmp"
                        || extension == ".tiff"
                    )
                    {
                        try
                        {
                            using (
                                var stream = new FileStream(
                                    droppedFilePath,
                                    FileMode.Open,
                                    FileAccess.Read
                                )
                            )
                            {
                                Image originalImage = Image.FromStream(stream);
                                Image resizedImage = new Bitmap(originalImage, new Size(1024, 768));
                                pictureBox2.Image = resizedImage;
                            }

                            // Pobranie metadanych
                            ImageMetadata metadata = ImageMetadata.LoadFromFile(droppedFilePath);
                            FNlabel.Text = "Nazwa pliku: " + metadata.FileName;
                            FSlabel.Text = "Rozmiar pliku: " + (metadata.FileSize / 1024) + " KB";
                            CDlabel.Text = "Data utworzenia: " + metadata.CreationDate.ToString();
                            UDlabel.Text = "Data wgrania: " + metadata.UploadDate.ToString();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Nie udało się załadować obrazu: " + ex.Message);
                            droppedFilePath = null; // Wyzeruj ścieżkę w przypadku błędu
                        }
                    }
                    else
                    {
                        MessageBox.Show("Plik nie jest obsługiwanym obrazem.");
                        droppedFilePath = null; // Wyzeruj ścieżkę dla nieobsługiwanego formatu
                    }
                }
            }
        }

        private void pictureBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Sprawdzenie, czy przynajmniej jeden z plików ma rozszerzenie jpg, jpeg lub inne obsługiwane
                if (
                    files.Any(file =>
                        file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                        || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                        || file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                        || file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                    )
                )
                {
                    e.Effect = DragDropEffects.Copy; // Dozwolone kopiowanie
                }
                else
                {
                    e.Effect = DragDropEffects.None; // Niedozwolony typ danych
                }
            }
            else
            {
                e.Effect = DragDropEffects.None; // Jeśli dane nie są plikami, zablokuj upuszczanie
            }
        }

        private void pictureBox2_Paint_1(object sender, PaintEventArgs e)
        {
            if (pictureBox2.Image == null) // Jeśli obrazek jest pusty
            {
                // Rysowanie tekstu
                string text = "Tu wrzuć plik";
                Font font = new Font("Arial", 14, FontStyle.Bold);
                Brush brush = Brushes.Gray;
                SizeF textSize = e.Graphics.MeasureString(text, font);

                // Rysowanie ikony (zakładam, że masz ikonę w zasobach)
                Image uploadIcon = Properties.Resources.Upload_to_Cloud; // Ikona z zasobów
                int iconX = (pictureBox2.Width - uploadIcon.Width) / 2; // Wyśrodkowanie ikony
                int iconY = (pictureBox2.Height - uploadIcon.Height - (int)textSize.Height - 10) / 2; // Wyśrodkowanie ikony z uwzględnieniem miejsca na tekst poniżej ikony

                // Rysowanie ikony
                e.Graphics.DrawImage(uploadIcon, iconX, iconY);

                // Ustawienie tekstu pod ikoną
                float textX = (pictureBox2.Width - textSize.Width) / 2; // Wyśrodkowanie tekstu
                float textY = iconY + uploadIcon.Height + 10; // Tekst pod ikoną, z odstępem 10 pikseli

                // Rysowanie tekstu
                e.Graphics.DrawString(text, font, brush, textX, textY);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(droppedFilePath))
            {
                MessageBox.Show("Nie załadowano żadnego pliku do przesłania.");
                return;
            }
            string blobConnectionString = ConfigurationManager
                .ConnectionStrings["BlobConnectionString"]
                .ConnectionString;
            string sqlConnectionString = ConfigurationManager
                .ConnectionStrings["SqlConnectionString"]
                .ConnectionString;
            string containerName = "documents";

            FileStream fileStream = null;
            string fileID = null;

            try
            {
                // Krok 1: Dodanie encji do SQL Server i uzyskanie fileID
                using (var sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    sqlConnection.Open();

                    // Przykładowa procedura składowana: dbo.InsertFileAndReturnID
                    // Procedura wstawia encję i zwraca fileID
                    using (
                        var sqlCommand = new SqlCommand("dbo.InsertFileAndReturnID", sqlConnection)
                    )
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        // Dodaj parametry procedury
                        sqlCommand.Parameters.AddWithValue(
                            "@FileName",
                            Path.GetFileName(droppedFilePath)
                        );
                        sqlCommand.Parameters.AddWithValue("@UploadDate", DateTime.UtcNow);
                        sqlCommand.Parameters.AddWithValue("@UserId", UserSession.UserId);

                        // Odczytaj wynik procedury
                        object result = sqlCommand.ExecuteScalar();
                        if (result != null)
                        {
                            fileID = result.ToString();
                        }
                        else
                        {
                            throw new Exception("Nie udało się uzyskać fileID z bazy danych.");
                        }
                    }
                }

                // Krok 2: Przesłanie pliku do Azure Blob Storage z metadanymi
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(
                    containerName
                );

                // Utworzenie kontenera, jeśli jeszcze nie istnieje
                containerClient.CreateIfNotExists();

                // Tworzenie klienta Blob dla przesyłanego pliku
                string blobName = Path.GetFileName(droppedFilePath);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                MessageBox.Show(
                    $"Plik {blobName} został pomyślnie przesłany do Azure Blob Storage z fileID: {fileID}"
                );

                // Otwórz plik do odczytu
                fileStream = File.OpenRead(droppedFilePath);

                // Prześlij plik
                blobClient.Upload(fileStream, true);

                // Dodanie metadanych do pliku (np. fileID)
                var metadata = new Dictionary<string, string>
                {
                    { "file_id", fileID },
                    { "user_id", UserSession.UserId.ToString() }, // Zapisanie fileID jako metadanych
                };
                blobClient.SetMetadata(metadata);

                string fileUri = blobClient.Uri.ToString();
                string uriBase64 = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(fileUri)
                );

                // 3. Aktualizacja rekordu w SQL
                DatabaseHelper dbHelper = new DatabaseHelper();
                bool isUpdated = dbHelper.UpdateFileRecord(fileID, uriBase64);

                if (isUpdated)
                {
                    MessageBox.Show(
                        "Plik został pomyślnie przesłany i zaktualizowany w bazie danych."
                    );
                }
                else
                {
                    MessageBox.Show("Aktualizacja bazy danych nie powiodła się.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas przesyłania pliku: " + ex.Message);
            }
            finally
            {
                // Zamknięcie strumienia pliku
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
        }
    }
}
