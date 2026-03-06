using System;
using System.Collections.Generic;
using DynamicReportSystem.Models;
using Microsoft.Data.SqlClient;

namespace DynamicReportSystem.Repositories
{
    public class SchemaRepository : ISchemaRepository
    {
        private readonly string _connectionString;

        public SchemaRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Invalid connection string.", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public List<TableInfo> GetBaseTables()
        {
            const string query = @"
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME";

            var tables = new List<TableInfo>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    tables.Add(new TableInfo { Name = reader.GetString(0) });
                }
            }

            return tables;
        }

        public List<ColumnInfo> GetColumns(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Invalid table name.", nameof(tableName));
            }

            const string query = @"
SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @TableName
ORDER BY ORDINAL_POSITION";

            var columns = new List<ColumnInfo>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@TableName", System.Data.SqlDbType.NVarChar, 128).Value = tableName;
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var columnName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                var dataType = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                if (!string.IsNullOrWhiteSpace(columnName))
                {
                    columns.Add(new ColumnInfo
                    {
                        Name = columnName,
                        DataType = dataType
                    });
                }
            }

            return columns;
        }
    }
}
