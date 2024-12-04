using System;
using System.Security.Cryptography;
using System.Text;
using ExpenseTracker.Models;
using Microsoft.Data.Sqlite;

namespace ExpenseTracker.Auth
{
    public class AuthHelper
    {
        private const string ConnectionString = "Data Source=ExpenseTracker.db;";

        public static void InitializeAuthDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        Salt TEXT NOT NULL,
                        CreatedAt DATETIME NOT NULL
                    )";
                
                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32]; // 32 bytes for a 256-bit salt
            RandomNumberGenerator.Fill(saltBytes); // Fill with secure random bytes
            return Convert.ToBase64String(saltBytes); // Convert to Base64 for storage or transmission
        }

       
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool RegisterUser(string username, string password)
        {
            try
            {
                string salt = GenerateSalt();
                string passwordHash = HashPassword(password, salt);

                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string insertQuery = @"
                        INSERT INTO Users (Username, PasswordHash, Salt, CreatedAt)
                        VALUES (@Username, @PasswordHash, @Salt, @CreatedAt)";

                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        command.Parameters.AddWithValue("@Salt", salt);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqliteException)
            {
                return false;
            }
        }

        public static bool ValidateLogin(string username, string password)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT PasswordHash, Salt FROM Users WHERE Username = @Username";

                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["PasswordHash"].ToString();
                            string salt = reader["Salt"].ToString();
                            string computedHash = HashPassword(password, salt);
                            return storedHash == computedHash;
                        }
                    }
                }
            }
            return false;
        }
    }
}
