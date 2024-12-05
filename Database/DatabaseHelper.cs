using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using ExpenseTracker.Models;

namespace ExpenseTracker.Database
{
    public class DatabaseHelper
    {
        private const string ConnectionString = "Data Source=ExpenseTracker.db;";

        public static void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                
                // Create table if it doesn't exist (removed DROP TABLE)
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Expenses (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date DATETIME NOT NULL,
                        Category TEXT NOT NULL,
                        DebitAmount DECIMAL(10,2) NOT NULL,
                        CreditAmount DECIMAL(10,2) NOT NULL,
                        Source TEXT NOT NULL,
                        Notes TEXT
                    )";
                
                using (var createCommand = new SqliteCommand(createTableQuery, connection))
                {
                    createCommand.ExecuteNonQuery();
                }
            }
        }

        public static void AddExpense(Expense expense)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = @"
                    INSERT INTO Expenses (Date, Category, DebitAmount, CreditAmount, Source, Notes) 
                    VALUES (@Date, @Category, @DebitAmount, @CreditAmount, @Source, @Notes)";
                
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Date", expense.Date);
                    command.Parameters.AddWithValue("@Category", expense.Category);
                    command.Parameters.AddWithValue("@DebitAmount", expense.DebitAmount);
                    command.Parameters.AddWithValue("@CreditAmount", expense.CreditAmount);
                    command.Parameters.AddWithValue("@Source", expense.Source);
                    command.Parameters.AddWithValue("@Notes", expense.Notes);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Expense> GetAllExpenses()
        {
            var expenses = new List<Expense>();
            
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Expenses ORDER BY Date DESC";
                
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expenses.Add(new Expense
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Category = reader["Category"].ToString(),
                                DebitAmount = Convert.ToDecimal(reader["DebitAmount"]),
                                CreditAmount = Convert.ToDecimal(reader["CreditAmount"]),
                                Source = reader["Source"].ToString(),
                                Notes = reader["Notes"]?.ToString()
                            });
                        }
                    }
                }
            }
            
            return expenses;
        }

        public static decimal GetTotalBalance()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string balanceQuery = @"
                    SELECT 
                        SUM(CreditAmount) - SUM(DebitAmount) as Balance 
                    FROM Expenses";
                
                using (var command = new SqliteCommand(balanceQuery, connection))
                {
                    var result = command.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        public static List<Expense> GetFilteredExpenses(int? year = null, int? month = null, string category = null, string source = null)
        {
            var expenses = new List<Expense>();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var conditions = new List<string>();
                var parameters = new List<SqliteParameter>();

                if (year.HasValue)
                {
                    conditions.Add("strftime('%Y', Date) = @Year");
                    parameters.Add(new SqliteParameter("@Year", year.Value.ToString()));
                }

                if (month.HasValue)
                {
                    conditions.Add("strftime('%m', Date) = @Month");
                    parameters.Add(new SqliteParameter("@Month", month.Value.ToString("D2")));
                }

                if (!string.IsNullOrEmpty(category))
                {
                    conditions.Add("Category = @Category");
                    parameters.Add(new SqliteParameter("@Category", category));
                }

                if (!string.IsNullOrEmpty(source))
                {
                    conditions.Add("Source = @Source");
                    parameters.Add(new SqliteParameter("@Source", source));
                }

                string whereClause = conditions.Count > 0 ? $"WHERE {string.Join(" AND ", conditions)}" : "";
                string query = $@"
                    SELECT Id, Date, Category, DebitAmount, CreditAmount, Source, Notes
                    FROM Expenses
                    {whereClause}
                    ORDER BY Date DESC";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expenses.Add(new Expense
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Date = DateTime.Parse(reader["Date"].ToString()),
                                Category = reader["Category"].ToString(),
                                DebitAmount = Convert.ToDecimal(reader["DebitAmount"]),
                                CreditAmount = Convert.ToDecimal(reader["CreditAmount"]),
                                Source = reader["Source"].ToString(),
                                Notes = reader["Notes"]?.ToString()
                            });
                        }
                    }
                }
            }
            return expenses;
        }
    }
}
