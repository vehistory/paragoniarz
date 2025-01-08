using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Azure.Storage.Blobs;

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

            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.Dock = DockStyle.Top;
            button3.MouseEnter += Button_MouseEnter;
            button3.MouseLeave += Button_MouseLeave;
            button1.MouseEnter += Button_MouseEnter;
            button1.MouseLeave += Button_MouseLeave;
            dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
        }

        private void gradientPanel(object sender, PaintEventArgs e, Panel panel)
        {
            Color startColor = Color.FromArgb(0, 64, 95);
            Color endColor = Color.FromArgb(64, 64, 64);

            using (
                LinearGradientBrush brush = new LinearGradientBrush(
                    panel.ClientRectangle,
                    startColor,
                    endColor,
                    45F
                )
            )
            {
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
            string nazwa = textBox8.Text;
            DateTime? dataOd = null;
            DateTime? dataDo = null;

            if (DateTime.TryParse(dateTimePicker1.Text, out DateTime parsedDataOd))
            {
                dataOd = parsedDataOd;
            }

            if (DateTime.TryParse(dateTimePicker2.Text, out DateTime parsedDataDo))
            {
                dataDo = parsedDataDo;
            }

            FinderManager finderManager = new FinderManager();
            string query = finderManager.CreateSearchQuery(_userId, nazwa, dataOd, dataDo);

            DatabaseHelper dbConnection = new DatabaseHelper();
            DataTable result = dbConnection.GetDataFromQuery(query);

            if (result.Rows.Count > 0)
            {
                tableLayoutPanel1.SuspendLayout();
                tableLayoutPanel1.Controls.Clear();
                tableLayoutPanel1.RowCount = 0;

                foreach (DataRow row in result.Rows)
                {
                    tableLayoutPanel1.RowCount++;
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
                    int currentRow = tableLayoutPanel1.RowCount - 1;
                    string url = System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(row["file_url"].ToString())
                    );

                    // Kolumna 0: Nazwa pliku
                    LinkLabel labelName = CreateLinkLabel(
                        row["original_name"].ToString(),
                        url,
                        Download_fileAsync,
                        ContentAlignment.MiddleLeft
                    );
                    tableLayoutPanel1.Controls.Add(labelName, 0, currentRow);

                    // Kolumna 1: Data dodania
                    Label labelDate = CreateLabel(row["timestamp"].ToString());
                    tableLayoutPanel1.Controls.Add(labelDate, 1, currentRow);

                    // Kolumna 2: Podgląd
                    Button buttonPreview = CreatePreviewButton(url);
                    tableLayoutPanel1.Controls.Add(buttonPreview, 2, currentRow);

                    // Kolumna 3: Usuń
                    Button buttonDelete = CreateDeleteButton(
                        row["id"].ToString(),
                        row["original_name"].ToString()
                    );
                    tableLayoutPanel1.Controls.Add(buttonDelete, 3, currentRow);
                }

                tableLayoutPanel1.ResumeLayout();
            }
            else
            {
                MessageBox.Show("Brak wyników dla podanych kryteriów.");
            }
        }

        private LinkLabel CreateLinkLabel(
            string text,
            string accessibleName,
            LinkLabelLinkClickedEventHandler clickHandler,
            ContentAlignment TextAlign = ContentAlignment.MiddleCenter
        )
        {
            LinkLabel label = new LinkLabel
            {
                Text = text,
                AccessibleName = accessibleName,
                TextAlign = TextAlign,
                LinkColor = Color.FromArgb(0, 192, 192),
                Font = new Font(SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Regular),
                Dock = DockStyle.Fill,
                AutoSize = false,
                AutoEllipsis = true,
            };
            label.LinkClicked += clickHandler;
            return label;
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 192, 192),
                Font = new Font(SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Regular),
                Dock = DockStyle.Fill,
                AutoSize = false,
                AutoEllipsis = true,
            };
        }

        private Button CreatePreviewButton(string url)
        {
            Button button = new Button
            {
                BackgroundImage = Properties.Resources.search_icon,
                BackgroundImageLayout = ImageLayout.Center,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(32, 32),
                Cursor = Cursors.Hand,
                Tag = url,
                FlatAppearance = { BorderSize = 0 },
            };
            button.Click += Preview_file;

            return button;
        }

        private Button CreateDeleteButton(string id, string fileName)
        {
            Button button = new Button
            {
                BackgroundImage = Properties.Resources.icons8_trash_26,
                BackgroundImageLayout = ImageLayout.Center,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(31, 33),
                Cursor = Cursors.Hand,
                Tag = fileName,
                AccessibleDescription = id,
                FlatAppearance = { BorderSize = 0 },
            };
            button.Click += button2_Click;

            return button;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 0;
        }

        public async Task DeleteFileFromBlobStorage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Nie znaleziono pliku do usunięcia.");
                return;
            }

            BlobContainerClient containerClient =
                DatabaseConnection.Instance.CreateBlobStorageConnection();

            try
            {
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();

                MessageBox.Show($"Plik {fileName} został pomyślnie usunięty.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania pliku z Azure: {ex.Message}");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            Button deleteButton = sender as Button;

            if (deleteButton != null)
            {
                string fileId = deleteButton.AccessibleDescription.ToString();
                string fileName = deleteButton.Tag.ToString();

                // Confirm deletion with the user
                DialogResult result = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć plik '{fileName}'?",
                    "Potwierdzenie usunięcia",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await DeleteFileFromBlobStorage(fileName);

                        DatabaseHelper dbHelper = new DatabaseHelper();
                        bool deletedFromDb = dbHelper.DeleteFileRecord(fileId, UserSession.UserId);

                        if (deletedFromDb)
                        {
                            MessageBox.Show(
                                "Plik został pomyślnie usunięty z Blob storage i bazy danych."
                            );

                            button3_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Plik został usunięty z Blob storage, ale wystąpił problem z usunięciem rekordu z bazy danych."
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Wystąpił błąd podczas usuwania pliku: {ex.Message}");
                    }
                }
            }
        }

        private void Preview_file(object sender, EventArgs e)
        {
            Button PreviewButton = sender as Button;
            if (PreviewButton != null)
            {
                string url = PreviewButton.Tag.ToString();
                try
                {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }
                    );
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

            string expandedPath = Environment.ExpandEnvironmentVariables(
                @"%userprofile%\Downloads"
            );
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
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(savePath) { UseShellExecute = true }
                    );
                }
                catch (Exception openEx)
                {
                    MessageBox.Show(
                        $"Plik został pobrany, ale nie udało się go otworzyć: {openEx.Message}"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}");
            }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = Color.FromArgb(0, 100, 148);
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = Color.Transparent;
            }
        }
    }
}
