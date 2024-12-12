using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Configuration;
using Paragoniarz;

public static class UserManager
{


    public static bool ValidateUser(string username,string password)
    {
        string connectionString = DatabaseHelper.ConnectionString;

        string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@username",username);
                cmd.Parameters.AddWithValue("@password",password); 

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

    public static bool IsUserExists(string username,string email)
    {
        string connectionString = DatabaseHelper.ConnectionString;
        string query = "SELECT COUNT(*) FROM dbo.users WHERE username = @username OR email = @email";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@username",username);
                cmd.Parameters.AddWithValue("@email",email);

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

    public static bool CreateUser(string username,string password,string email)
    {
        string connectionString = DatabaseHelper.ConnectionString;
        string query = "INSERT INTO dbo.users (username, password, email) VALUES (@username, @password, @email)";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@username",username);
                cmd.Parameters.AddWithValue("@password",password); 
                cmd.Parameters.AddWithValue("@email",email);

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
