using System.Configuration;

using System.Data.SqlClient;

namespace Paragoniarz
{
    public sealed class DatabaseConnection
    {
        // Statyczna zmienna przechowująca jedyną instancję klasy
        private static readonly DatabaseConnection _instance = new DatabaseConnection();


        // Publiczna statyczna właściwość umożliwiająca dostęp do instancji
        public static DatabaseConnection Instance
        {
            get
            {
                return _instance;
            }
        }

        // Metoda do utworzenia połączenia z bazą danych
        public SqlConnection CreateConnection()
        {
            // Pobieramy connection string z pliku konfiguracyjnego (app.config)
            string connectionString = ConfigurationManager.ConnectionStrings["ParagoniarzConnectionString"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}
