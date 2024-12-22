using Azure.Storage.Blobs; // Dodaj odpowiednią przestrzeń nazw
using System;
using System.Data.SqlClient;
using System.Configuration;

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
        try
        {
            // Pobieramy connection string z pliku konfiguracyjnego (app.config)
            string connectionString = ConfigurationManager.ConnectionStrings["ParagoniarzConnectionString"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string for database is not found.");
            }

            return new SqlConnection(connectionString);
        }
        catch (Exception ex)
        {
            // Obsługuje wyjątek i może rzucić go dalej lub logować
            throw new ApplicationException("Failed to create database connection.",ex);
        }
    }
    public string GetBlobStorageConnectionString()
    {
        string blobStorageConnectionString = ConfigurationManager.ConnectionStrings["BlobStorageConnectionString"]?.ConnectionString;
        if (string.IsNullOrEmpty(blobStorageConnectionString))
        {
            throw new InvalidOperationException("Connection string for Blob Storage is not found.");
        }
        return blobStorageConnectionString;
    }
    public BlobContainerClient CreateBlobStorageConnection()
    {
        // Pobieramy connection string z pliku konfiguracyjnego (app.config)
        string blobStorageConnectionString = ConfigurationManager.ConnectionStrings["BlobStorageConnectionString"].ConnectionString;

        // Tworzymy klienta BlobServiceClient z connection stringa
        BlobServiceClient blobServiceClient = new BlobServiceClient(blobStorageConnectionString);

        // Uzyskujemy dostęp do kontenera (zmień nazwę kontenera na swoją)
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("documents");

        return containerClient;
    }
}
