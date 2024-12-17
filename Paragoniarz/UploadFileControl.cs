using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            pictureBox2.Image = null;
            FNlabel.Text = "Nazwa pliku: ";
            FSlabel.Text = "Rozmiar pliku: ";
            CDlabel.Text = "Data utworzenia: ";
            UDlabel.Text = "Data wgrania: ";
        }

        private void pictureBox2_DragDrop(object sender,DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    string filePath = files[0]; // Pierwszy plik
                    string extension = Path.GetExtension(filePath).ToLower();

                    // Sprawdzamy, czy jest to plik graficzny
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" ||
                        extension == ".gif" || extension == ".bmp" || extension == ".tiff")
                    {
                        try
                        {
                            // Załaduj obraz do PictureBox
                            pictureBox2.Image = Image.FromFile(filePath);
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
    }
}
