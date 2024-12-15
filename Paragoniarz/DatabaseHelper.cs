using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;





namespace Paragoniarz
{
    public class DatabaseHelper
    {
        //public static string ConnectionString
        //{
        //    get
        //    {
        //        return ConfigurationManager.ConnectionStrings["ParagoniarzConnectionString"].ConnectionString;
        //    }
        //}


        
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

        // Walidacja użytkownika
        public bool ValidateUser(string username,string password)
        {
            string hashedPassword = HashPassword(password);

            string query = "SELECT COUNT(*) FROM dbo.Users WHERE username = @username AND password = @password";

            using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query,conn);
                    cmd.Parameters.AddWithValue("@username",username);
                    cmd.Parameters.AddWithValue("@password",hashedPassword);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas logowania: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
