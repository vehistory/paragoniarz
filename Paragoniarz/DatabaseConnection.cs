﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using Azure.Storage.Blobs;

namespace Paragoniarz
{
    public sealed class DatabaseConnection
    {
        private static readonly Lazy<DatabaseConnection> _lazyInstance =
            new Lazy<DatabaseConnection>(() => new DatabaseConnection());

        private readonly string _connectionString;

        private DatabaseConnection()
        {
            try
            {
                _connectionString = ConfigurationManager
                    .ConnectionStrings["SqlConnectionString"]
                    .ConnectionString;

                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new ConfigurationErrorsException(
                        "Connection string 'SqlConnectionString' is missing or empty in the configuration file."
                    );
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine($"Configuration error: {ex.Message}");
                throw;
            }
        }

        public static DatabaseConnection Instance => _lazyInstance.Value;

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public void ExecuteQuery(Action<SqlConnection> action)
        {
            using (SqlConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    action(connection);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL error when executing query: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error when executing query: {ex.Message}");
                    throw;
                }
            }
        }

        public T ExecuteQuery<T>(Func<SqlConnection, T> func)
        {
            using (SqlConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    return func(connection);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL error when executing query: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error when executing query: {ex.Message}");
                    throw;
                }
            }
        }

        public BlobContainerClient CreateBlobStorageConnection()
        {
            string blobStorageConnectionString = GetBlobStorageConnectionString();

            BlobServiceClient blobServiceClient = new BlobServiceClient(
                blobStorageConnectionString
            );
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(
                "documents"
            );

            return containerClient;
        }

        public string GetBlobStorageConnectionString()
        {
            string blobStorageConnectionString = ConfigurationManager
                .ConnectionStrings["BlobConnectionString"]
                ?.ConnectionString;

            if (string.IsNullOrEmpty(blobStorageConnectionString))
            {
                throw new InvalidOperationException(
                    "Connection string for Blob Storage is not found."
                );
            }

            return blobStorageConnectionString;
        }
    }
}
