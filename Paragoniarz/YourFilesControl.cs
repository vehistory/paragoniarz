using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Paragoniarz
{
    public partial class YourFilesControl : UserControl
    {
        private static readonly HttpClient client = new HttpClient();

        public YourFilesControl()
        {
            InitializeComponent();
            tableFiles.Visible = false;
        }

        private async void button3_Click(object sender, System.EventArgs e)
        {
            string phraseSearch = textBoxPhrase.Text;
            string personSearch = textBoxPerson.Text;

            string searchServiceEndpoint = ConfigurationManager
                .ConnectionStrings["SearchServiceEndpoint"]
                ?.ConnectionString;
            string apiKey = ConfigurationManager
                .ConnectionStrings["SearchServiceApiKey"]
                ?.ConnectionString;
            string indexName = ConfigurationManager
                .ConnectionStrings["SearchServiceIndexName"]
                ?.ConnectionString;

            if (
                string.IsNullOrEmpty(searchServiceEndpoint)
                || string.IsNullOrEmpty(apiKey)
                || string.IsNullOrEmpty(indexName)
            )
            {
                Console.WriteLine(searchServiceEndpoint);
                Console.WriteLine(apiKey);
                Console.WriteLine(indexName);
                MessageBox.Show(
                    "Brak wymaganych ustawień konfiguracyjnych. Sprawdź plik konfiguracyjny aplikacji."
                );
                return;
            }

            var searchRequest = new
            {
                search = phraseSearch,
                filter = BuildFilter(UserSession.UserId, personSearch),
                count = true,
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(searchRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync(
                    $"{searchServiceEndpoint}/indexes/{indexName}/docs/search?api-version=2024-11-01-preview",
                    content
                );

                tableFiles.SuspendLayout();
                tableFiles.Controls.Clear();
                tableFiles.RowCount = 0;

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();

                    JObject searchResults = JObject.Parse(jsonResult);

                    tableFiles.Visible = true;
                    tableFiles.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

                    foreach (var result in searchResults["value"])
                    {
                        tableFiles.RowCount++;
                        tableFiles.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

                        int currentRow = tableFiles.RowCount - 1;
                        string originalPath = result["metadata_storage_path"].ToString();
                        string url = DecodeBase64Url(originalPath);

                        LinkLabel labelName = CreateLinkLabel(
                            result["metadata_storage_name"].ToString(),
                            url,
                            Download_fileAsync,
                            ContentAlignment.MiddleLeft
                        );
                        tableFiles.Controls.Add(labelName, 0, currentRow);

                        Label labelDate = CreateLabel(result["metadata_creation_date"].ToString());
                        tableFiles.Controls.Add(labelDate, 1, currentRow);

                        Button buttonPreview = CreatePreviewButton(url);
                        tableFiles.Controls.Add(buttonPreview, 2, currentRow);
                    }

                    tableFiles.ResumeLayout();

                    if (searchResults["@odata.count"].Value<int>() == 0)
                    {
                        MessageBox.Show("Brak wyników dla podanych kryteriów.");
                    }
                }
                else
                {
                    tableFiles.Visible = false;
                    MessageBox.Show($"Błąd podczas wyszukiwania: {response.StatusCode}");
                }
            }
        }

        private LinkLabel CreateLinkLabel(
            string text,
            string url,
            LinkLabelLinkClickedEventHandler clickHandler,
            ContentAlignment TextAlign = ContentAlignment.MiddleCenter
        )
        {
            LinkLabel label = new LinkLabel
            {
                Text = text,
                AccessibleName = url,
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

        private static string DecodeBase64Url(string originalPath)
        {
            string url;
            string cleanBase64 = originalPath.TrimEnd('=');

            // Determine padding based on the last character
            int lastChar = int.Parse(cleanBase64[cleanBase64.Length - 1].ToString());
            string paddedBase64 = cleanBase64.Substring(0, cleanBase64.Length - 1);

            if (lastChar == 1)
            {
                paddedBase64 += "=";
            }
            else if (lastChar == 2)
            {
                paddedBase64 += "==";
            }
            else if (lastChar == 3) // Probably not possible
            {
                paddedBase64 += "===";
            }
            // If lastChar is 0, we don't add any padding

            try
            {
                byte[] decodedBytes = Convert.FromBase64String(paddedBase64);
                url = Encoding.UTF8.GetString(decodedBytes);
                url = url.Trim();

                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    return url;
                }
                else
                {
                    Console.WriteLine($"Zdekodowany URL nie jest poprawnym URI: {url}");
                    return originalPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd dekodowania: {ex.Message}");
                return originalPath;
            }
        }

        private static string BuildFilter(int userId, string personSearch)
        {
            var filters = new List<string>();

            filters.Add($"user_id eq '{userId}'");

            if (!string.IsNullOrWhiteSpace(personSearch))
            {
                var searchTerms = personSearch
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(term => term.Trim());

                if (searchTerms.Any())
                {
                    var peopleSearchFilter = string.Join(
                        " or ",
                        searchTerms.Select(term => $"people/any(p: p eq '{term}')")
                    );
                    filters.Add($"({peopleSearchFilter})");
                }
            }

            return string.Join(" and ", filters);
        }

        private void textBoxPhrase_KeyDown(object sender,KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_Click(sender,e);  // Wywołaj metodę kliknięcia przycisku
            }
        }

        private void textBoxPerson_KeyDown(object sender,KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_Click(sender,e);  // Wywołaj metodę kliknięcia przycisku
            }
        }
    }
}
