using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;

namespace Paragoniarz
{
    public class DatabaseHelper
    {
        // Zmienna connectionString, używana w całej klasie
        string connectionString = "Server=paragoniarz.database.windows.net;Port=1433;Database=paragoniarz_db;Uid=p4ragoniarzadmin;Pwd=zaq1@WSX123;SslMode=Required;";


        // Testowe połączenie z bazą danych
        //public void TestConnection()
        //{
        //    using (MySqlConnection conn = new MySqlConnection(connectionString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            Console.WriteLine("Połączenie udane!");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Błąd połączenia: {ex.Message}");
        //        }
        //    }
        //}

        public void TestConnection()
        {
            Console.WriteLine("Przygotowanie do nawiązania połączenia...");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    Console.WriteLine("Sprawdzanie poprawności connection stringa...");
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        Console.WriteLine("Błąd: Connection string jest pusty lub null!");
                        return;
                    }

                    Console.WriteLine("Connection string: " + connectionString);
                    Console.WriteLine("Rozpoczynanie otwierania połączenia...");

                    conn.Open();
                    Console.WriteLine("Połączenie otwarte pomyślnie!");

                    Console.WriteLine("Sprawdzanie statusu połączenia...");
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        Console.WriteLine("Połączenie z bazą danych jest aktywne.");
                    }
                    else
                    {
                        Console.WriteLine("Połączenie z bazą danych nie zostało otwarte.");
                    }
                }
                catch (MySqlException mysqlEx)
                {
                    Console.WriteLine($"MySqlException: {mysqlEx.Message}");
                    Console.WriteLine($"Kod błędu MySql: {mysqlEx.Number}");
                    Console.WriteLine($"Źródło błędu: {mysqlEx.Source}");
                }
                catch (InvalidOperationException invalidOpEx)
                {
                    Console.WriteLine($"InvalidOperationException: {invalidOpEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Nieznany błąd: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine("Próba zamknięcia połączenia...");
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                        Console.WriteLine("Połączenie zamknięte.");
                    }
                    else
                    {
                        Console.WriteLine("Połączenie było już zamknięte lub nigdy nie zostało otwarte.");
                    }
                }
            }
        }



        // Sprawdzanie, czy użytkownik lub email są już zajęte
        public bool IsUsernameOrEmailTaken(string username,string email)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username OR email = @email";
                    using (var cmd = new MySqlCommand(query,connection))
                    {
                        cmd.Parameters.AddWithValue("@username",username);
                        cmd.Parameters.AddWithValue("@email",email);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0; // Zwróci true, jeśli użytkownik lub email są już zajęte
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd: " + ex.Message);
                    return false; // W przypadku błędu zakłada, że użytkownik nie istnieje
                }
            }
        }

        // Metoda do dodawania użytkownika z hasłem w formie haszowanej
        public void InsertUser(string username,string email,string password)
        {
            string hashedPassword = HashPassword(password); // Haszowanie hasła

            string query = "INSERT INTO dbo.users (username, password, email) VALUES (@username, @password, @email)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query,conn);
                    cmd.Parameters.AddWithValue("@username",username);
                    
                    cmd.Parameters.AddWithValue("@password",hashedPassword); // Hasło zahaszowane
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
                // Konwertowanie danych na tablicę bajtów
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Konwertowanie tablicy bajtów na ciąg
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
            string hashedPassword = HashPassword(password); // Haszowanie hasła
            MessageBox.Show($"Hashed password: {hashedPassword}"); // Wyświetl hash, aby upewnić się, że jest poprawny

            string query = "SELECT COUNT(*) FROM Users WHERE username = @username AND password = @password";
            MessageBox.Show($"Query: {query}"); // Wyświetl zapytanie SQL

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query,conn);
                    cmd.Parameters.AddWithValue("@username",username);
                    cmd.Parameters.AddWithValue("@password",hashedPassword); // Weryfikacja hasła w postaci haszowanej

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; // Jeśli znaleziono użytkownika, zwróci true
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
