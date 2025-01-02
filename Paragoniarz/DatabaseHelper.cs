using Azure.Storage.Blobs;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paragoniarz
{
    public class DatabaseHelper
    {
        public static class SessionData
        {
            public static int UserId { get; set; }
            public static string UserName { get; set; }
        }


        // Sprawdzanie, czy użytkownik lub email są już zajęte
        public bool IsUsernameOrEmailTaken(string username, string email)
        {
            using (var connection = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    connection.Open();
                    string query =
                        "SELECT COUNT(*) FROM dbo.Users WHERE username = @username OR email = @email";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd: " + ex.Message);
                    return false;
                }
            }
        }
        // Metoda do dodawania użytkownika z hasłem w formie haszowanej
        public void InsertUser(string username, string email, string password)
        {
            string hashedPassword = HashPassword(password); // Haszowanie hasła

            string query =
                "INSERT INTO dbo.Users (username, password, email) VALUES (@username, @password, @email)";

            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@email", email);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Użytkownik został pomyślnie dodany.");
                    }
                    else
                    {
                        MessageBox.Show("Błąd podczas rejestracji użytkownika.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas dodawania użytkownika: {ex.Message}");
                }
            }
        }
        // Metoda haszująca hasło
        private string HashPassword(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // Metoda ValidateUser zwraca teraz krotkę (idUser, username)
        public int? ValidateUser(string username, string password)
        {
            string hashedPassword = HashPassword(password);

            // Zapytanie SQL teraz zwraca idUser
            string query = "SELECT id FROM dbo.Users WHERE username = @username AND password = @password";
            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);

                    object result = cmd.ExecuteScalar(); // Zwraca pojedynczy wynik, idUser

                    if (result != null)
                    {
                        return Convert.ToInt32(result); // Zwraca idUser, jeśli użytkownik istnieje
                    }
                    else
                    {
                        return null; // Zwraca null, jeśli nie znaleziono użytkownika
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas logowania: {ex.Message}");
                    return null;
                }
            }
        }

        // Metoda do wykonywania zapytań SQL
        public void ExecuteQuery(string query)
        {
            // Pobieranie instancji połączenia z bazy danych
            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    // Otwarcie połączenia
                    conn.Open();

                    // Tworzenie obiektu SqlCommand do wykonania zapytania
                    SqlCommand cmd = new SqlCommand(query,conn);

                    // Wykonanie zapytania i zwrócenie liczby zmodyfikowanych wierszy
                    int result = cmd.ExecuteNonQuery();

                    // Sprawdzenie rezultatu zapytania
                    if (result > 0)
                    {
                        MessageBox.Show("Operacja zakończona pomyślnie.");
                    }
                    else
                    {
                        MessageBox.Show("Operacja zakończona niepowodzeniem.");
                    }
                }
                catch (Exception ex)
                {
                    // Obsługa błędu
                    MessageBox.Show($"Błąd podczas wykonywania zapytania: {ex.Message}");
                }
                finally
                {
                    // Zamykanie połączenia (jest to obsługiwane automatycznie przez using)
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

        // Metoda do pobierania danych z bazy danych
        public DataTable GetDataFromQuery(string query)
        {
            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable; // Zwraca wynik jako DataTable
                    }
                }
            }
        }

        public bool UpdateFileRecord(string fileID, string fileUriBase64)
        {
            using (var sqlConnection = DatabaseConnection.Instance.CreateConnection())
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("dbo.UpdateFileUri", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    // Dodanie parametrów
                    sqlCommand.Parameters.AddWithValue("@FileID", fileID);
                    sqlCommand.Parameters.AddWithValue("@FileUriBase64", fileUriBase64);

                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    return rowsAffected > 0; // Zwraca true, jeśli rekord został zaktualizowany
                }
            }
        }

        public string GetFileNameFromDatabase(int userId)
        {
            string query = "SELECT fileName FROM dbo.Files WHERE userId = @userId"; // Załóżmy, że masz tabelę 'Files'

            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query,conn);
                    cmd.Parameters.AddWithValue("@userId",userId);

                    object result = cmd.ExecuteScalar(); // Zwraca tylko jedną wartość (nazwa pliku)

                    if (result != null)
                    {
                        return result.ToString(); // Zwraca nazwę pliku
                    }
                    else
                    {
                        return null; // Zwraca null, jeśli nie znaleziono pliku
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas pobierania nazwy pliku: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task DeleteFileFromBlobStorage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Nie podano nazwy pliku do usunięcia.");
                return;
            }

            // Wyszukiwanie i usuwanie pliku z Azure Blob Storage
            string connectionString = DatabaseConnection.Instance.CreateBlobStorageConnection().ToString();
            string containerName = "documents";

            BlobContainerClient containerClient = new BlobContainerClient(connectionString,containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            try
            {
                // Usuwanie pliku z Blob Storage
                await blobClient.DeleteIfExistsAsync();
                MessageBox.Show($"Plik {fileName} został pomyślnie usunięty.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania pliku z Azure: {ex.Message}");
            }
        }
    }
}
