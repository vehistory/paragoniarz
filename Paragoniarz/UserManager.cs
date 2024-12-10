using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public static class UserManager
{


    public static bool ValidateUser(string username,string password)
    {
        string connectionString = "Server=paragoniarz.database.windows.net; Port=1433; Database=paragoniarz_db; Uid=p4ragoniarzadmin; Pwd=zaq1@WSX123;";

        string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@username",username);
                cmd.Parameters.AddWithValue("@password",password); // Pamiętaj o odpowiednim zabezpieczeniu hasła (np. hashowanie)

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
        string connectionString = "Server=paragoniarz.database.windows.net; Port=1433; Database=paragoniarz_db; Uid=p4ragoniarzadmin; Pwd=zaq1@WSX123;";
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
                return count > 0;  // Jeśli wynik jest większy niż 0, użytkownik już istnieje
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
        string connectionString = "Server=paragoniarz.database.windows.net; Port=1433; Database=paragoniarz_db; Uid=p4ragoniarzadmin; Pwd=zaq1@WSX123;";
        string query = "INSERT INTO dbo.users (username, password, email) VALUES (@username, @password, @email)";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@username",username);
                cmd.Parameters.AddWithValue("@password",password);  // Wartość hasła może wymagać hashowania
                cmd.Parameters.AddWithValue("@email",email);

                int result = cmd.ExecuteNonQuery();
                return result > 0;  // Jeśli użytkownik zostanie dodany, metoda zwróci true
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas tworzenia użytkownika: {ex.Message}");
                return false;
            }
        }
    }







}
