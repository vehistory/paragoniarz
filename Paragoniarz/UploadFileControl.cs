using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Paragoniarz
{
    public partial class UploadFileControl : UserControl
    {
        public UploadFileControl()
        {
            InitializeComponent();
            pictureBox2.AllowDrop = true;
        }

        private void button2_Click(object sender,EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
                pictureBox2.Image = null;
            }
            FNlabel.Text = "Nazwa pliku: ";
            FSlabel.Text = "Rozmiar pliku: ";
            CDlabel.Text = "Data utworzenia: ";
            UDlabel.Text = "Data wgrania: ";
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }


        private void pictureBox2_DragDrop(object sender,DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    string filePath = files[0]; // Pierwszy plik


                    FileInfo fileInfo = new FileInfo(filePath);

                    long maxSizeInBytes = 2 * 1024 * 1024; // 2 MB
                    if (fileInfo.Length > maxSizeInBytes)
                    {
                        MessageBox.Show("Plik jest zbyt duży. Maksymalny rozmiar to 2 MB.");
                        return;
                    }

                    string extension = Path.GetExtension(filePath).ToLower();






                    // Sprawdzamy, czy jest to plik graficzny
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" ||
                        extension == ".gif" || extension == ".bmp" || extension == ".tiff")
                    {
                        try
                        {
                            // Załaduj obraz do PictureBox
                            using (var stream = new FileStream(filePath,FileMode.Open,FileAccess.Read))
                            {
                                Image originalImage = Image.FromStream(stream);

                                // Tworzenie nowego obrazu o mniejszych wymiarach
                                Image resizedImage = new Bitmap(originalImage,new Size(1024,768)); // zmniejsz wymiary zgodnie z potrzebą
                                pictureBox2.Image = resizedImage;
                            }

                            // Pobranie metadanych przez klasę ImageMetadata
                            ImageMetadata metadata = ImageMetadata.LoadFromFile(filePath);
                            // Wyświetlanie metadanych w odpowiednich labelach
                            FNlabel.Text = "Nazwa pliku: " + metadata.FileName;
                            FSlabel.Text = "Rozmiar pliku: " + (metadata.FileSize / 1024) + " KB"; // Konwersja na KB
                            CDlabel.Text = "Data utworzenia: " + metadata.CreationDate.ToString();
                            UDlabel.Text = "Data wgrania: " + metadata.UploadDate.ToString();


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Nie udało się załadować obrazu: " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Plik nie jest obsługiwanym obrazem.");
                    }
                }
            }
        }

        private void pictureBox2_DragEnter(object sender,DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                // Sprawdzenie, czy przynajmniej jeden z plików ma rozszerzenie jpg, jpeg lub inne obsługiwane
                if (files.Any(file => file.EndsWith(".jpg",StringComparison.OrdinalIgnoreCase) ||
                                      file.EndsWith(".jpeg",StringComparison.OrdinalIgnoreCase) ||
                                      file.EndsWith(".png",StringComparison.OrdinalIgnoreCase) ||
                                      file.EndsWith(".gif",StringComparison.OrdinalIgnoreCase)))
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

        private void pictureBox2_Paint_1(object sender,PaintEventArgs e)
        {
            if (pictureBox2.Image == null) // Jeśli obrazek jest pusty
            {
                // Rysowanie tekstu
                string text = "Tu wrzuć plik";
                Font font = new Font("Arial",14,FontStyle.Bold);
                Brush brush = Brushes.Gray;
                SizeF textSize = e.Graphics.MeasureString(text,font);

                // Rysowanie ikony (zakładam, że masz ikonę w zasobach)
                Image uploadIcon = Properties.Resources.Upload_to_Cloud; // Ikona z zasobów
                int iconX = (pictureBox2.Width - uploadIcon.Width) / 2; // Wyśrodkowanie ikony
                int iconY = (pictureBox2.Height - uploadIcon.Height - (int) textSize.Height - 10) / 2; // Wyśrodkowanie ikony z uwzględnieniem miejsca na tekst poniżej ikony

                // Rysowanie ikony
                e.Graphics.DrawImage(uploadIcon,iconX,iconY);

                // Ustawienie tekstu pod ikoną
                float textX = (pictureBox2.Width - textSize.Width) / 2; // Wyśrodkowanie tekstu
                float textY = iconY + uploadIcon.Height + 10; // Tekst pod ikoną, z odstępem 10 pikseli

                // Rysowanie tekstu
                e.Graphics.DrawString(text,font,brush,textX,textY);
            }
        }
    }
}
