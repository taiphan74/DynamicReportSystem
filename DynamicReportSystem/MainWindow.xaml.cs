using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DynamicReportSystem.Configuration;
using DynamicReportSystem.Models;
using DynamicReportSystem.Repositories;
using DynamicReportSystem.Services;

namespace DynamicReportSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDatabaseExplorerService? _databaseExplorerService;
        private readonly ISqlQueryBuilderService _sqlQueryBuilderService = new SqlQueryBuilderService();
        private readonly Dictionary<string, List<ColumnInfo>> _tableColumns = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _allowedTables = new(StringComparer.OrdinalIgnoreCase);

        private string? _selectedTableName;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var connectionString = AppConfiguration.GetConnectionString();
                var schemaRepository = new SchemaRepository(connectionString);
                var tableDataRepository = new TableDataRepository(connectionString);
                _databaseExplorerService = new DatabaseExplorerService(schemaRepository, tableDataRepository);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Cannot initialize application configuration.\nDetails: {ex.Message}",
                    "Configuration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadExplorerOnStartup();
        }

        private void LoadExplorerOnStartup()
        {
            if (_databaseExplorerService is null)
            {
                return;
            }

            try
            {
                var rootNode = _databaseExplorerService.BuildExplorerTree();
                tvExplorer.ItemsSource = new List<ExplorerNode> { rootNode };

                _allowedTables.Clear();
                _tableColumns.Clear();

                foreach (var tableNode in rootNode.Children.Where(n => n.NodeType == ExplorerNodeType.Table))
                {
                    _allowedTables.Add(tableNode.Name);
                    _tableColumns[tableNode.Name] = tableNode.Children
                        .Where(c => c.NodeType == ExplorerNodeType.Column)
                        .Select(c => new ColumnInfo { Name = c.Name, DataType = c.DataType ?? string.Empty })
                        .ToList();
                }

                if (rootNode.Children.Count == 0)
                {
                    MessageBox.Show("No tables found in the selected database.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot load schema explorer.\nDetails: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tvExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tvExplorer.SelectedItem is not ExplorerNode selectedNode)
            {
                return;
            }

            if (selectedNode.NodeType != ExplorerNodeType.Table)
            {
                return;
            }

            _selectedTableName = selectedNode.Name;
            txtSelectedTable.Text = $"Bảng đã chọn: {_selectedTableName}";
        }

        private void btnGenerateSql_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_selectedTableName))
                {
                    MessageBox.Show("Please double-click a table in Schema Explorer first.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var options = new QueryBuilderOptions
                {
                    SelectedTable = _selectedTableName,
                    IncludeSelect = chkSelect.IsChecked == true,
                    UseWhere = chkWhere.IsChecked == true,
                    UseGroupBy = chkGroupBy.IsChecked == true,
                    UseOrderBy = false,
                    Top = 0
                };

                var allowedColumns = _tableColumns.TryGetValue(_selectedTableName, out var columns)
                    ? columns.Select(c => c.Name).ToList()
                    : new List<string>();

                var result = _sqlQueryBuilderService.BuildSelectQuery(options, _allowedTables.ToList(), allowedColumns);
                txtGeneratedSql.Text = result.Sql;

                if (result.Warnings.Count > 0)
                {
                    MessageBox.Show(string.Join(Environment.NewLine, result.Warnings), "Builder Warnings", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot generate SQL.\nDetails: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtGeneratedSql.Clear();
            chkSelect.IsChecked = true;
            chkWhere.IsChecked = false;
            chkGroupBy.IsChecked = false;
            _selectedTableName = null;
            txtSelectedTable.Text = "Bảng đã chọn: (chưa chọn)";
        }
    }
}
