using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DynamicReportSystem.Repositories
{
    public class TableDataRepository : ITableDataRepository
    {
        private readonly string _connectionString;

        public TableDataRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Invalid connection string.", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public DataTable GetTopRows(string tableName, int top)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Invalid table name.", nameof(tableName));
            }

            if (top <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(top), "Top must be greater than 0.");
            }

            var safeTableName = tableName.Replace("]", "]]", StringComparison.Ordinal);
            var query = $"SELECT TOP {top} * FROM [{safeTableName}]";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            using var adapter = new SqlDataAdapter(command);

            var result = new DataTable();
            connection.Open();
            adapter.Fill(result);

            return result;
        }
    }
}
