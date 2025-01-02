using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

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

        // Sprawdzanie, czy użytkownik lub email są już zajęte
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

        // Walidacja użytkownika
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

                    // Dodanie parametrów
                    sqlCommand.Parameters.AddWithValue("@FileID", fileID);
                    sqlCommand.Parameters.AddWithValue("@FileUriBase64", fileUriBase64);

                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    return rowsAffected > 0; // Zwraca true, jeśli rekord został zaktualizowany
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
    }
}
