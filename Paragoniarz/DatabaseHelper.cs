using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Azure.Storage.Blobs;

namespace Paragoniarz
{
    public class DatabaseHelper
    {
        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }

        public bool IsUsernameOrEmailTaken(string username, string email)
        {
            try
            {
                return DatabaseConnection.Instance.ExecuteQuery(connection =>
                {
                    string query =
                        "SELECT COUNT(*) FROM dbo.Users WHERE username = @username OR email = @email";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas sprawdzania użytkownika/email: {ex.Message}");
                return false;
            }
        }

        public void InsertUser(string username, string email, string password)
        {
            string hashedPassword = HashPassword(password);

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

        public User ValidateUser(string username, string password)
        {
            string hashedPassword = HashPassword(password);

            try
            {
                return DatabaseConnection.Instance.ExecuteQuery(connection =>
                {
                    string query =
                        "SELECT id, username, email FROM dbo.Users WHERE username = @username AND password = @password";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Email = reader.GetString(2),
                                };
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas logowania: {ex.Message}");
                return null;
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

                    sqlCommand.Parameters.AddWithValue("@FileID", fileID);
                    sqlCommand.Parameters.AddWithValue("@FileUriBase64", fileUriBase64);

                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

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
                        return dataTable;
                    }
                }
            }
        }

        public string GetFileNameFromDatabase(int userId)
        {
            string query = "SELECT fileName FROM dbo.Files WHERE userId = @userId";

            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                    else
                    {
                        return null;
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

            string connectionString = DatabaseConnection
                .Instance.CreateBlobStorageConnection()
                .ToString();
            string containerName = "documents";

            BlobContainerClient containerClient = new BlobContainerClient(
                connectionString,
                containerName
            );
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            try
            {
                await blobClient.DeleteIfExistsAsync();
                MessageBox.Show($"Plik {fileName} został pomyślnie usunięty.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania pliku z Azure: {ex.Message}");
            }
        }

        public bool DeleteFileRecord(string fileId, int userId)
        {
            try
            {
                return DatabaseConnection.Instance.ExecuteQuery(connection =>
                {
                    string query =
                        "DELETE FROM [dbo].[files] WHERE id = @FileID AND user_id = @UserID";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FileID", fileId);
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania pliku: {ex.Message}");
                return false;
            }
        }

        private static string HashPassword(string rawData)
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
    }
}
