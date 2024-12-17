using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Paragoniarz
{
    public partial class Form2 : Form
    {
        // Deklaracja kontrolek
        private UserControl currentControl = null;
        private UploadFileControl uploadFileControl;
        private YourFilesControl yourFilesControl;
        private FindFileControl findFileControl;
        



        public Form2(string username)
        {
            InitializeComponent();

            pictureBox2.AllowDrop = true;
            uploadFileControl = new UploadFileControl();
            yourFilesControl = new YourFilesControl();
            findFileControl = new FindFileControl();
            panel4.Controls.Clear();
            





            // Zaokrąglij rogi okna 
            WindowHelper.SetWindowRoundCorners(this,20);

            // Umożliw przesuwanie okna 
            WindowHelper.EnableWindowDragging(panel1,this);

            pnlNav.Height = sendFile.Height;
            pnlNav.Top = sendFile.Top;
            pnlNav.Left = sendFile.Left;
            sendFile.BackColor = Color.FromArgb(46,51,73);
           
            // Sprawdź, czy nazwa użytkownika została przekazana i przypisz ją do labela
            if (!string.IsNullOrEmpty(username))
            {
                label1.Text = "Witaj " + username + "!";
            }
            else
            {
                label1.Text = "Witaj nieznajomy!";
            }

        }
        private void ShowControl(UserControl control)
        {
            // Usuń aktualną kontrolkę, jeśli jakaś jest
            if (currentControl != null)
            {
                panel4.Controls.Remove(currentControl);
            }

            // Dodaj nową kontrolkę
            panel4.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            currentControl = control;
        }


        private void button1_Click(object sender,EventArgs e)
        {
            Environment.Exit(0);
        }
       
        private void SetNavigationPanel(Control clickedButton)
        {
            pnlNav.Height = clickedButton.Height;
            pnlNav.Top = clickedButton.Top;
            pnlNav.Left = clickedButton.Left;
            clickedButton.BackColor = Color.FromArgb(46,51,73);


        }



        private void button3_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(sendFile);
            ShowControl(uploadFileControl);
        }


        private void button4_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(yoursFile);
            ShowControl(findFileControl);


        }

        private void button5_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(findFile);
            ShowControl(yourFilesControl);



        }

        private void button6_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(button6);
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            form1.Location = this.Location;
            form1.Show();
            this.Hide();
        }

        private void SetButtonDefaultColor(Button button)
        {
            button.BackColor = Color.FromArgb(24,30,54);
        }
        private void button3_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(sendFile);
        }


        private void button4_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(yoursFile);
        }

        private void button5_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(findFile);
        }

        private void button6_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(button6);
        }

        private void Form2_Load(object sender,EventArgs e)
        {

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

        private void pictureBox2_Paint(object sender,PaintEventArgs e)
        {
            if (pictureBox2.Image == null) // Jeśli obrazek jest pusty
            {
               
                string text = "Tu wrzuć plik";
                Font font = new Font("Arial",14,FontStyle.Bold);
                Brush brush = Brushes.Gray;
                SizeF textSize = e.Graphics.MeasureString(text,font);

                // Rysowanie ikony 
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


        private void button2_Click_1(object sender,EventArgs e)
        {
            pictureBox2.Image = null;
            FNlabel.Text = "Nazwa pliku: ";
            FSlabel.Text = "Rozmiar pliku: ";
            CDlabel.Text = "Data utworzenia: ";
            UDlabel.Text = "Data wgrania: ";

        }
    }
}
