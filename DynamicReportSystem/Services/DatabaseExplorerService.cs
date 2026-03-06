using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DynamicReportSystem.Models;
using DynamicReportSystem.Repositories;

namespace DynamicReportSystem.Services
{
    public class DatabaseExplorerService : IDatabaseExplorerService
    {
        private const int PreviewTopRows = 100;

        private readonly ISchemaRepository _schemaRepository;
        private readonly ITableDataRepository _tableDataRepository;
        private readonly HashSet<string> _knownTables = new(StringComparer.OrdinalIgnoreCase);

        public DatabaseExplorerService(ISchemaRepository schemaRepository, ITableDataRepository tableDataRepository)
        {
            _schemaRepository = schemaRepository ?? throw new ArgumentNullException(nameof(schemaRepository));
            _tableDataRepository = tableDataRepository ?? throw new ArgumentNullException(nameof(tableDataRepository));
        }

        public ExplorerNode BuildExplorerTree()
        {
            var root = new ExplorerNode
            {
                Name = "Tables",
                NodeType = ExplorerNodeType.Root
            };

            var tableNames = LoadTableNames();
            foreach (var tableName in tableNames)
            {
                var tableNode = new ExplorerNode
                {
                    Name = tableName,
                    TableName = tableName,
                    NodeType = ExplorerNodeType.Table
                };

                var columns = LoadColumns(tableName);
                foreach (var column in columns)
                {
                    tableNode.Children.Add(new ExplorerNode
                    {
                        Name = column.Name,
                        DataType = column.DataType,
                        TableName = tableName,
                        NodeType = ExplorerNodeType.Column
                    });
                }

                root.Children.Add(tableNode);
            }

            return root;
        }

        public List<string> LoadTableNames()
        {
            var tableNames = _schemaRepository
                .GetBaseTables()
                .Select(t => t.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();

            _knownTables.Clear();
            foreach (var tableName in tableNames)
            {
                _knownTables.Add(tableName);
            }

            return tableNames;
        }

        public List<ColumnInfo> LoadColumns(string tableName)
        {
            EnsureValidTable(tableName);
            return _schemaRepository.GetColumns(tableName);
        }

        public DataTable GetPreviewData(string tableName)
        {
            EnsureValidTable(tableName);
            return _tableDataRepository.GetTopRows(tableName, PreviewTopRows);
        }

        private void EnsureValidTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Table name is required.", nameof(tableName));
            }

            if (_knownTables.Count == 0)
            {
                throw new InvalidOperationException("Table list is empty. Please load table list first.");
            }

            if (!_knownTables.Contains(tableName))
            {
                throw new InvalidOperationException("Invalid table selected. Please choose from loaded table list.");
            }
        }
    }
}
