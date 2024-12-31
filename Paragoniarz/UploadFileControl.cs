using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Paragoniarz
{
    public partial class UploadFileControl : UserControl
    {
        private const long MaxFileSizeBytes = 2 * 1024 * 1024; // 2 MB
        private const string ContainerName = "documents";
        private const string DefaultDropText = "Tu wrzuć plik";
        private readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".bmp",
            ".tiff",
            ".pdf",
        };

        private string _droppedFilePath;

        public UploadFileControl()
        {
            InitializeComponent();
            SetupPictureBox();
        }

        private void SetupPictureBox()
        {
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
                pictureBox2.Image.Dispose();
                pictureBox2.Image = null;
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

        private void PictureBox2_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length == 0)
            {
                return;
            }

            _droppedFilePath = files[0];

            ProcessDroppedFile();
        }

        private void ProcessDroppedFile()
        {
            if (!ValidateFile())
                return;

            try
            {
                LoadFilePreview();
                UpdateFileMetadata();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się załadować pliku: {ex.Message}");
                _droppedFilePath = null;
            }
        }

        private bool ValidateFile()
        {
            var fileInfo = new FileInfo(_droppedFilePath);
            if (fileInfo.Length > MaxFileSizeBytes)
            {
                MessageBox.Show("Plik jest zbyt duży. Maksymalny rozmiar to 2 MB.");
                _droppedFilePath = null;
                return false;
            }

            string extension = Path.GetExtension(_droppedFilePath).ToLower();
            if (!AllowedExtensions.Contains(extension))
            {
                MessageBox.Show("Plik nie jest obsługiwanym formatem.");
                _droppedFilePath = null;
                return false;
            }

            return true;
        }

        private void LoadFilePreview()
        {
            string extension = Path.GetExtension(_droppedFilePath).ToLower();
            if (extension == ".pdf")
            {
                pictureBox2.Image = Properties.Resources.pdf_icon;
            }
            else
            {
                using (
                    var stream = new FileStream(_droppedFilePath, FileMode.Open, FileAccess.Read)
                )
                {
                    Image originalImage = Image.FromStream(stream);
                    Image resizedImage = new Bitmap(originalImage, new Size(1024, 768));
                    pictureBox2.Image = resizedImage;
                }
            }
        }

        private void UpdateFileMetadata()
        {
            var metadata = ImageMetadata.LoadFromFile(_droppedFilePath);
            UpdateLabels(metadata);
        }

        private void PictureBox2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect =
                e.Data.GetDataPresent(DataFormats.FileDrop)
                && ((string[])e.Data.GetData(DataFormats.FileDrop)).Any(file =>
                    AllowedExtensions.Contains(Path.GetExtension(file).ToLower())
                )
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
        }

        private void PictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                return;
            }

            DrawPlaceholderContent(e.Graphics);
        }

        private void DrawPlaceholderContent(Graphics graphics)
        {
            var text = DefaultDropText;
            Font font = new Font("Arial", 14, FontStyle.Bold);
            Brush brush = Brushes.Gray;
            SizeF textSize = graphics.MeasureString(text, font);

            var uploadIcon = Properties.Resources.Upload_to_Cloud;
            var iconX = (pictureBox2.Width - uploadIcon.Width) / 2;
            var iconY = (pictureBox2.Height - uploadIcon.Height - (int)textSize.Height - 10) / 2;

            graphics.DrawImage(uploadIcon, iconX, iconY);

            var textX = (pictureBox2.Width - textSize.Width) / 2;
            var textY = iconY + uploadIcon.Height + 10;

            graphics.DrawString(text, font, brush, textX, textY);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_droppedFilePath))
            {
                MessageBox.Show("Nie załadowano żadnego pliku do przesłania.");
                return;
            }

            try
            {
                var blobConnectionString = GetConnectionString("BlobConnectionString");

                var fileId = InsertFileRecordAndGetId();
                var blobUri = UploadFileToBlobStorage(blobConnectionString, fileId);
                UpdateFileRecord(fileId, blobUri);

                MessageBox.Show("Plik został pomyślnie przesłany i zaktualizowany w bazie danych.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas przesyłania pliku: {ex.Message}");
            }
        }

        private string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException(
                    $"Brak lub nieprawidłowy ciąg połączenia: {name}; Sprawdź App.config"
                );
            }
            return connectionString;
        }

        private string InsertFileRecordAndGetId()
        {
            return DatabaseConnection.Instance.ExecuteQuery(connection =>
            {
                using (var sqlCommand = new SqlCommand("dbo.InsertFileAndReturnID", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue(
                        "@FileName",
                        Path.GetFileName(_droppedFilePath)
                    );
                    sqlCommand.Parameters.AddWithValue("@UploadDate", DateTime.UtcNow);
                    sqlCommand.Parameters.AddWithValue("@UserId", UserSession.UserId);

                    var result = sqlCommand.ExecuteScalar();
                    if (result == null)
                    {
                        throw new Exception("Nie udało się uzyskać fileID z bazy danych.");
                    }
                    return result.ToString();
                }
            });
        }

        private string UploadFileToBlobStorage(string blobConnectionString, string fileId)
        {
            var blobServiceClient = new BlobServiceClient(blobConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            containerClient.CreateIfNotExists();

            var blobName = Path.GetFileName(_droppedFilePath);
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var fileStream = File.OpenRead(_droppedFilePath))
            {
                var contentType = GetContentType(_droppedFilePath);
                var options = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                    Conditions = new BlobRequestConditions { IfNoneMatch = ETag.All },
                    Metadata = new Dictionary<string, string>
                    {
                        { "file_id", fileId },
                        { "user_id", UserSession.UserId.ToString() },
                    },
                };

                blobClient.Upload(fileStream, options);
            }

            return blobClient.Uri.ToString();
        }

        private void UpdateFileRecord(string fileId, string blobUri)
        {
            var uriBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(blobUri));
            var dbHelper = new DatabaseHelper();
            if (!dbHelper.UpdateFileRecord(fileId, uriBase64))
            {
                throw new Exception("Aktualizacja bazy danych nie powiodła się.");
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".pdf")
            {
                return "application/pdf";
            }
            else if (extension == ".jpg" || extension == ".jpeg")
            {
                return "image/jpeg";
            }
            else if (extension == ".png")
            {
                return "image/png";
            }
            else
            {
                return "application/octet-stream";
            }
        }

        private void UpdateLabels(ImageMetadata metadata)
        {
            FNlabel.Text = "Nazwa pliku: " + metadata.FileName;
            FSlabel.Text = "Rozmiar pliku: " + (metadata.FileSize / 1024) + " KB";
            CDlabel.Text = "Data utworzenia: " + metadata.CreationDate.ToString();
            UDlabel.Text = "Data wgrania: " + metadata.UploadDate.ToString();
        }
    }
}
