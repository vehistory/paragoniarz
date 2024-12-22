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

        // Sprawdzanie, czy użytkownik lub email są już zajęte
        public bool IsUsernameOrEmailTaken(string username,string email)  
        {
            using (var connection = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM dbo.Users WHERE username = @username OR email = @email";
                    using (var cmd = new SqlCommand(query,connection))
                    {
                        cmd.Parameters.AddWithValue("@username",username);
                        cmd.Parameters.AddWithValue("@email",email);

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
        public void InsertUser(string username,string email,string password)
        {
            string hashedPassword = HashPassword(password); // Haszowanie hasła

            string query = "INSERT INTO dbo.Users (username, password, email) VALUES (@username, @password, @email)";

            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query,conn);
                    cmd.Parameters.AddWithValue("@username",username);
                    cmd.Parameters.AddWithValue("@password",hashedPassword);
                    cmd.Parameters.AddWithValue("@email",email);

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
        public int? ValidateUser(string username,string password)
        {
            string hashedPassword = HashPassword(password);

            // Zapytanie SQL teraz zwraca idUser
            string query = "SELECT id FROM dbo.Users WHERE username = @username AND password = @password";
            


            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query,conn);
                    cmd.Parameters.AddWithValue("@username",username);
                    cmd.Parameters.AddWithValue("@password",hashedPassword);

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


        // Metoda do pobierania danych z bazy danych
        public DataTable GetDataFromQuery(string query)
        {
            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query,conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;  // Zwraca wynik jako DataTable
                    }
                }
            }
        }
    }
}
