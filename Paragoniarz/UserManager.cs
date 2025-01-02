using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Paragoniarz;

public static class UserManager
{
    public static bool ValidateUser(string username, string password)
    {
        string query =
            "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";

        using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd połączenia z bazą danych: {ex.Message}");
                return false;
            }
        }
    }

    public static bool IsUserExists(string username, string email)
    {
        string query =
            "SELECT COUNT(*) FROM dbo.users WHERE username = @username OR email = @email";

        using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@email", email);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas sprawdzania użytkownika: {ex.Message}");
                return false;
            }
        }
    }

    public static bool CreateUser(string username, string password, string email)
    {
        string query =
            "INSERT INTO dbo.users (username, password, email) VALUES (@username, @password, @email)";

        using (SqlConnection conn = DatabaseConnection.Instance.CreateConnection())
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@email", email);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas tworzenia użytkownika: {ex.Message}");
                return false;
            }
        }
    }
}
